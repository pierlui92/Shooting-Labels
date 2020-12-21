import os
import argparse
import numpy as np
from open3d import *
from utils import *
import math

parser = argparse.ArgumentParser()
parser.add_argument("--input_path", type=str, default='pcd.ply', help="input ply")
parser.add_argument("--output_path", type=str, default='../splits/', help="output ply")
parser.add_argument("--classes", type=str, default='../../unity/Assets/CSV/ColorConfiguration.csv', help="classes csv")
parser.add_argument("--category_mapping", type=str, default='category_mapping.tsv', help="mapping matterport")
parser.add_argument("--id_mapping", type=str, default='mapping_nyu402eigen13.txt', help="mapping from id to id")
parser.add_argument("--visualize", action='store_true', help="visualize_results")
parser.add_argument("--dataset", type=str, default='none',choices=['matterport','kitti', 'none'], help="dataset")
parser.add_argument("--from_encoding", type=str, default='id',choices=['id','color'], help="from format")
parser.add_argument("--to_encoding", type=str, default='color',choices=['id','color'], help="to format")
# parser.add_argument("--object_type", type=str, default='pointcloud',choices=['mesh','pointcloud'], help="mesh or pointcloud")
args=parser.parse_args()

print("Converting")

def distance(c1, c2):
    (r1,g1,b1) = c1.split(" ")
    r1=float(r1)
    g1=float(g1)
    b1=float(b1)
    (r2,g2,b2) = c2
    return math.sqrt((r1 - r2)**2 + (g1 - g2) ** 2 + (b1 - b2) **2)

if args.from_encoding == 'id' and args.to_encoding == 'color':
    id2color,color2id,id2color_lut,_,num_classes = decode_classes(args.classes)
    vertices, faces, ids, colors = read_ply_with_custom_fields(args.input_path, read_colors= False)

    if args.dataset == 'matterport':
        cat2id = decode_categories(args.category_mapping,num_classes)
        ids = [cat2id[i] for i in ids]
    
    sem_color_face=[id2color_lut[i] for i in ids]
    
    colors[faces]=np.stack([sem_color_face,sem_color_face,sem_color_face],axis=1)
    write_ply_with_custom_fields(args.output_path, vertices, colors, faces, ids, multiplier_rgb=1)

if args.from_encoding == 'color' and args.to_encoding == 'id':
    id2color,color2id,id2color_lut,_,num_classes = decode_classes(args.classes)
    vertices, faces, ids, colors = read_ply_with_custom_fields(args.input_path, read_ids=False, read_colors= True if args.from_encoding == 'color' else False)
    colors_keys = list(color2id.keys())

    ids = np.zeros([faces.shape[0]])
    for i in range(faces.shape[0]):
        color_face = colors[faces[i]][0]
        key_color = str(color_face[0]) + " " + str(color_face[1]) + " " + str(color_face[2])
        
        if key_color in color2id.keys():
            ids[i] = color2id[key_color]
        else:
            closest_color = sorted(colors_keys, key=lambda color: distance(color, color_face))[0]
            ids[i] = color2id[closest_color]

    write_ply_with_custom_fields(args.output_path,vertices,colors,faces,ids,multiplier_rgb=1)

if args.from_encoding == 'id' and args.to_encoding == 'id':
    vertices, faces, ids, colors = read_ply_with_custom_fields(args.input_path, read_colors= False)
    lines=open(args.id_mapping).readlines()
    id2id=[0]*len(lines)
    for l in lines:
        l=l.strip()
        id2id[int(l.split(",")[0])]=int(l.split(",")[1])
    ids_new = [id2id[i] for i in ids]
    write_ply_with_custom_fields(args.output_path,vertices,colors,faces,ids_new)

print("Done.")
