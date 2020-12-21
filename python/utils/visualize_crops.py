from open3d import *
import argparse
import numpy as np
import os

parser = argparse.ArgumentParser()
parser.add_argument("--input_path", type=str, default='pcd.ply', help="input ply")
args=parser.parse_args()

all=[]
for f in os.listdir(args.input_path):
    if f.endswith(".ply"):
        tmp = read_point_cloud(os.path.join(args.input_path,f))  
        minArray = tmp.get_min_bound()
        maxArray = tmp.get_max_bound()
        dimension = dimension_split = np.abs(maxArray-minArray)
        tmp = crop_point_cloud(tmp, minArray+dimension/15, maxArray+dimension/15)
        all.append(tmp)

draw_geometries(all)