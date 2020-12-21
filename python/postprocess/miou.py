import numpy as np
from plyfile import PlyData, PlyElement
from numpy.linalg import norm
import argparse
import os
from open3d import *
import math
from utils.utils import *

parser = argparse.ArgumentParser()
parser.add_argument("--input_path", type=str, help="input ply")
parser.add_argument("--gt_path", type=str, help="gt ply")
parser.add_argument("--output_path", type=str, default='results.txt', help="output txt results")
parser.add_argument("--ignore_labels", type=int,  nargs='+', default=[0,2,11,14], help="ids to ignore in evaluation")
parser.add_argument("--classes", type=str, default='../unity/Assets/CSV/ColorConfiguration.csv', help="classes csv")
parser.add_argument("--category_mapping", type=str, default='utils/category_mapping.tsv', help="mapping matterport")
parser.add_argument("--threshold_confidence", type=float, default=0, help="confidence threshold")
args=parser.parse_args()

def area(a, b, c):
    return 0.5*norm(np.cross(b-a,c-a))

id2color,color2id,id2color_lut,names,num_classes = decode_classes(args.classes)
#cat2id = decode_categories(args.category_mapping,num_classes)

conf_mat=np.zeros([num_classes,num_classes]) ## [GT, PRED]
conf_mat_sup=np.zeros([num_classes,num_classes]) ## [GT, PRED]

total_area=0
total_faces=0
labeled_area=0
labeled_faces=0

for idx,f in enumerate(os.listdir(args.input_path)):
    input_path = os.path.join(args.input_path,f)
    gt_path = os.path.join(args.gt_path,f)

    print(idx,f,"Reading GT       ", end='\r')
    vertices_gt, faces_gt, ids_gt, _ = read_ply_with_custom_fields(gt_path,read_colors=False)
    #ids_gt=[cat2id[i] for i in ids_gt_tmp.tolist()]
    centroids_gt=np.mean(vertices_gt[faces_gt],axis=1)

    print(idx,f,"Reading Annotated", end='\r')
    if args.threshold_confidence>0:
        vertices_pred, faces_pred, ids_pred, _, confidence = read_ply_with_custom_fields(input_path,read_colors=False, read_confidence=True)
        centroids_pred=np.mean(vertices_pred[faces_pred],axis=1)
    else:
        vertices_pred, faces_pred, ids_pred, _ = read_ply_with_custom_fields(input_path,read_colors=False)
        centroids_pred=np.mean(vertices_pred[faces_pred],axis=1)
        confidence = np.ones_like(ids_pred)

    pcd_pred = PointCloud()
    pcd_pred.points = Vector3dVector(centroids_pred)
    tree = KDTreeFlann(pcd_pred)

    print(idx,f,"Updating mIoU    ", end='\r')
    for c in range(centroids_gt.shape[0]):
        k, idx, _ = tree.search_knn_vector_3d(Vector3dVector([centroids_gt[c]])[0],1)
        idx=idx[0]
        face_vertices=vertices_gt[faces_gt[idx]]
        cl_gt = ids_gt[c]
        cl_pred = ids_pred[idx]
        
        ### UPDATE TOTAL ###
        sup = area(face_vertices[0],face_vertices[1],face_vertices[2])
        total_area += sup
        total_faces += 1
        
        ### UPDATE CONFUSION MATRIX ###
        if (cl_gt not in args.ignore_labels) and (cl_pred not in args.ignore_labels) and (confidence[idx] >= args.threshold_confidence):
            conf_mat_sup[cl_gt,cl_pred] += sup*10000
            conf_mat[cl_gt,cl_pred] += 1
            labeled_area += sup
            labeled_faces += 1

### MIOU ###
FN_TP=np.sum(conf_mat,axis=0)
FP_TP=np.sum(conf_mat,axis=1)
TP=np.diag(conf_mat)
IoU=TP/(FN_TP+FP_TP-TP)
mIoU=np.mean([i for i in IoU if not math.isnan(i)])*100

### MIOU AREA ###
FN_TP=np.sum(conf_mat_sup,axis=0)
FP_TP=np.sum(conf_mat_sup,axis=1)
TP=np.diag(conf_mat_sup)
IoU_sup=TP/(FN_TP+FP_TP-TP)
mIoU_sup=np.mean([i for i in IoU_sup if not math.isnan(i)])*100

### PERCENTAGE COMPLETE ###
perc = labeled_faces / total_faces * 100
perc_area = labeled_area / total_area * 100

print(IoU, "\n")
print(IoU_sup, "\n")
print("mIoU: %.2f mIoU Area %.2f percentage complete %.2f percentage area %.2f" % (mIoU, mIoU_sup, perc, perc_area))

### LOGGING ###
f = open(args.output_path,"w")
for i in range(len(IoU)):
    f.write(names[i] + "\t") 
f.write("mIoU\tperc\n")

for i in range(len(IoU)):
    f.write(str(IoU[i]) + "\t")
f.write(str(perc) + "\t" + str(mIoU) + "\n")        

f.write("Area\n")        

for i in range(len(IoU_sup)):
    f.write(str(IoU_sup[i]) + "\t")
f.write(str(perc_area) + "\t" + str(mIoU_sup) + "\n")        
f.close()