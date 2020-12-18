import numpy as np
import argparse
from open3d import *
from utils.utils import *

def expand(input_path, output_path, classes, ignore_labels, neighbours, expand_colors=True, threshold_confidence=0, weight_by_conf=False):
    id2color,color2id,id2color_lut,names,num_classes = decode_classes(classes)

    print("Reading Annotated     ", end='\r')
    if threshold_confidence > 0:
        vertices_pred, faces_pred, ids_pred, colors_pred, conf_pred = read_ply_with_custom_fields(input_path, read_colors=expand_colors, read_confidence=True)
    else:
        vertices_pred, faces_pred, ids_pred, colors_pred = read_ply_with_custom_fields(input_path, read_colors=expand_colors)
    
    centroids_pred=np.mean(vertices_pred[faces_pred],axis=1)

    cond = np.ones_like(ids_pred).astype(bool)
    for l in ignore_labels:
        cond= np.logical_and((ids_pred != l),cond)
    
    if threshold_confidence > 0:
        d = conf_pred[cond].shape[0]
        cond = np.logical_and(conf_pred > threshold_confidence,cond)
        if weight_by_conf:
            conf_labeled = conf_pred[cond] ### WEIGHTING CONFIDENCE### 
            print("Percentage deleted by confidence: ", (1-conf_labeled.shape[0]/d)*100)
    
    unlabeled_points=centroids_pred[np.logical_not(cond)]
    labeled_points=centroids_pred[cond]
    labels=ids_pred[cond]

    pcd_labeled = PointCloud()
    pcd_labeled.points = Vector3dVector(labeled_points)
    tree_labeled = KDTreeFlann(pcd_labeled)

    pcd_all = PointCloud()
    pcd_all.points = Vector3dVector(centroids_pred)
    tree_all = KDTreeFlann(pcd_all)

    print("Expanding to Unlabeled", end='\r')
    for p in range(unlabeled_points.shape[0]):
        k,idx,_=tree_labeled.search_knn_vector_3d(Vector3dVector([unlabeled_points[p]])[0],neighbours)
        
        if weight_by_conf:
            histogram = [0] * num_classes
            for i in idx:
                histogram[labels[i]] += conf_labeled[i] 
        else:
            histogram=np.histogram(labels[idx],bins=range(num_classes))[0]

        new_label=np.argmax(histogram)
        k,idx,_=tree_all.search_knn_vector_3d(Vector3dVector([unlabeled_points[p]])[0],1)
        ids_pred[idx[0]] = new_label
        if expand_colors:
            color_new=id2color_lut[new_label]
            colors_pred[faces_pred[idx[0]]]=np.stack([color_new,color_new,color_new],axis=0)

    write_ply_with_custom_fields(output_path,vertices_pred,colors_pred,faces_pred,ids_pred, multiplier_rgb=1)

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--input_path", type=str, help="input ply")
    parser.add_argument("--output_path", type=str, default='../results.ply', help="output txt results")
    parser.add_argument("--classes", type=str, default='../../unity/Assets/CSV/ColorConfiguration.csv', help="classes csv")
    parser.add_argument("--neighbours", type=int, default=12, help="k of knn")
    parser.add_argument("--ignore_labels", nargs='+', type = int, default=[0,14], help="ignore labels")
    parser.add_argument("--not_expand_colors", dest='expand_colors' , action='store_false', help="not expand colors")
    parser.add_argument("--threshold_confidence", type=float, default=0, help="expand labels also to not confidence")
    parser.add_argument("--weight_by_confidence", action='store_true', help="weight expansion histogram by confidence")
    args=parser.parse_args()

    if not os.path.exists(args.output_path):
        os.makedirs(args.output_path)

    for f in os.listdir(args.input_path):
        if f.endswith(".ply"):
            print("Expanding ", f)
            expand(os.path.join(args.input_path, f), os.path.join(args.output_path,f) , args.classes, args.ignore_labels, args.neighbours, args.expand_colors, args.threshold_confidence, args.weight_by_confidence)