import numpy as np
from open3d import *
import time
from numpy.linalg import norm
import math
import argparse
from scipy.stats import mode

parser = argparse.ArgumentParser()
parser.add_argument("--input_path_voxel_ply", type=str, default='/data/pier_data/3DReconstruct/2011_09_30_drive_0020/blender_labels/blender_labels.ply', help="input ply")
parser.add_argument("--input_path_original_ply", type=str, default='/data/pier_data/3DReconstruct/2011_09_30_drive_0020/Pcd/pcd.ply', help="input ply")
parser.add_argument("--transform", type=str, default='transform.txt', help="transform 4x4 from ply to voxel")
parser.add_argument("--classes", type=str, default='/data/pier_data/3DReconstruct/2011_09_30_drive_0020/classes.csv')
parser.add_argument("--output_path", type=str, default='/data/pier_data/3DReconstruct/2011_09_30_drive_0020/blender_labels/blender_labels_points.ply', help="output folder")
parser.add_argument("--num_classes", type=int, default=20, help="number of classes including void")
args=parser.parse_args()

def decode_pose(path):
    line=open(path).readline()
    splits = line.split(" ")
    pose=np.zeros([4,4])
    pose[3,3]=1
    for idx,s in enumerate(splits):
        pose[idx//4, idx%4]=float(s)
    print(pose)
    return pose

sizeVoxel = 0.25
num_classes = args.num_classes
nearestPoints = 12

pose = decode_pose(args.transform)

dictio_id2color={}
dictio_color2id={}

cs=open(args.classes).readlines()

for c in cs:
    id=int(c.strip().split(" ")[0])
    r,g,b=c.strip().split(" ")[-1].split(",")

    r=float(r)
    g=float(g)
    b=float(b)

    dictio_id2color[id]=c.strip().split(" ")[-1].replace(","," ")
    dictio_color2id[int((r+g+b)//3)]=id

print(dictio_id2color,dictio_color2id)

pcd = read_point_cloud(args.input_path_original_ply)
voxelized = read_triangle_mesh(args.input_path_voxel_ply)

#draw_geometries([pcd,voxelized])

#voxelized.transform(pose)

vertices=np.asarray(voxelized.vertices)
triangles=np.asarray(voxelized.triangles)

labels=np.mean(np.asarray(voxelized.vertex_colors),axis=-1)
centroids_labels,_=mode((labels[triangles]*255).astype(int),axis=-1)
#print(centroids_labels)
centroids_labels=centroids_labels[:,0]

centroids=np.mean(vertices[triangles],axis=1)
pcd_centroids = PointCloud()
pcd_centroids.points = Vector3dVector(np.asarray(centroids))
pcd_tree = KDTreeFlann(pcd_centroids)

f = open(args.output_path,'w+')
f.write("ply\n")
f.write("format ascii 1.0\n")
f.write("element vertex %d\n" % len(pcd.points))
f.write("property float x\n")
f.write("property float y\n")
f.write("property float z\n")
f.write("property uchar red\n")
f.write("property uchar green\n")
f.write("property uchar blue\n")
f.write("property uchar id_category\n")
f.write("end_header\n")

for i,point in enumerate(pcd.points):
    print(i, "/", len(pcd.points) ,end='\r')
    k, idx, _ = pcd_tree.search_knn_vector_3d(point,nearestPoints)
    listOfClasses = [0]*num_classes

    for id in idx:		
        if centroids_labels[id] in dictio_color2id.keys():
            label=dictio_color2id[centroids_labels[id]]
            listOfClasses[label] = listOfClasses[label] + 1
    
    idClassMax = np.argmax(np.asarray(listOfClasses))

    f.write("%f %f %f " % (point[0],point[1],point[2]))
    f.write("%s %d\n" % (dictio_id2color[idClassMax],idClassMax))

f.close()

pcd = read_point_cloud(args.output_path)
draw_geometries([pcd])