from open3d import *
import os
import argparse
import numpy as np
from plyfile import PlyData, PlyElement
from numpy.linalg import norm
import argparse
import os
from open3d import *
import math
from utils.utils import *

def softmax(x):
    return np.exp(x) / np.sum(np.exp(x))

def freq(x):
    return x/np.sum(x)

def entropy(x, num_classes):
    return -np.sum(x*np.log(x+1e-10)/np.log(num_classes))

def integrate(input_list, output_path,classes, confidence=True):
    id2color,color2id,id2color_lut,names,num_classes = decode_classes(classes)

    vertices_all=[]
    faces_all=[]
    ids_all=[]

    num = 0
    for idx,f in enumerate(input_list):
        print("Loading ", f)
        if f.endswith(".ply"):
            num += 1
            vertices, faces, ids, colors= read_ply_with_custom_fields(f)
            vertices_all.append(vertices)
            faces_all.append(faces + vertices.shape[0]*idx)
            ids_all.append(ids)
    
    centroids=np.mean(vertices[faces],axis=1)

    vertices_all = np.concatenate(vertices_all, axis=0)
    faces_all = np.concatenate(faces_all,axis=0)
    ids_all = np.concatenate(ids_all,axis=0)

    centroids_all=np.mean(vertices_all[faces_all],axis=1)
    pcd_all = PointCloud()
    pcd_all.points = Vector3dVector(centroids_all)
    tree_all = KDTreeFlann(pcd_all)

    if confidence:
        confidence_faces=[]
    
    for i in range(centroids.shape[0]):
        k,idx,_=tree_all.search_knn_vector_3d(Vector3dVector([centroids[i]])[0],num)
        histogram=np.histogram(ids_all[idx],bins=range(num_classes))[0]
        
        new_label=np.argmax(histogram)
        ids[i] = new_label

        color_new=id2color_lut[new_label]
        colors[faces[i]]=np.stack([color_new,color_new,color_new],axis=0)

        if confidence:    

            #conf = np.max(freq(histogram)) 
            #conf = 1 - entropy(softmax(histogram),num_classes)
            conf = 1 - entropy(freq(histogram),num_classes)
            confidence_faces.append(conf)

    write_ply_with_custom_fields(output_path,vertices, colors, faces, ids, confidence= confidence_faces if confidence else None, multiplier_rgb=1)

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--input_path", type=str, help="input folder with ply")
    parser.add_argument("--output_path", type=str, default='../results/integration/', help="output foolder")
    parser.add_argument("--classes", type=str, default='../unity/Assets/CSV/ColorConfiguration.csv', help="classes csv")
    parser.add_argument("--not_confidence", dest='confidence', action='store_false' , help="not compute confidence")
    args=parser.parse_args()

    if not os.path.exists(args.output_path):
        os.makedirs(args.output_path)

    players=[]
    for player in os.listdir(args.input_path):
        players.append(player)

    for f in os.listdir(os.path.join(args.input_path, players[0], "merge")):
        if f.endswith(".ply"):
            print("Elaborating ", f)
            input_list = [os.path.join(args.input_path, p, "merge", f) for p in players]
            integrate(input_list, os.path.join(args.output_path,f) , args.classes, confidence=args.confidence)