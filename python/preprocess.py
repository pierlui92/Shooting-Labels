from open3d import *
import argparse
import numpy as np
import os
import time
import preprocess.split_3D as split_3D
import subprocess

parser = argparse.ArgumentParser()
parser.add_argument("--input_path", type=str, default='pcd.ply', help="input ply")
parser.add_argument("--output_path", type=str, default='./splits/', help="output folder")
parser.add_argument("--split_x", type=int, default=1, help="split along x")
parser.add_argument("--split_y", type=int, default=1, help="split along y")
parser.add_argument("--split_z", type=int, default=1, help="split along z")
parser.add_argument("--mode_split", dest='mode', type=str, default='dimension',choices=['number','dimension'], help="split by by number or dimension of chunks")
parser.add_argument("--object_type", type=str, default='pointcloud',choices=['mesh','pointcloud'], help="mesh or pointcloud")
parser.add_argument("--skip_split", action='store_true', help="skip splitting operation")
### VOXELIZE PARAMETERS ###
parser.add_argument("-template_path","--template_path", dest='path_template', type=str, default="../resources/template.blend", help="empty blender template file")
parser.add_argument("-step_0","--step_0", dest='step_0', type=float, default= 0.25,  help="discretization step LOD 0")
parser.add_argument("-step_1","--step_1", dest='step_1', type=float, default= 0.5,  help="discretization step LOD 1")
parser.add_argument("-step_2","--step_2", dest='step_2', type=float, default= 1.0,  help="discretization step LOD 2")
parser.add_argument("-threshold_voxels","--threshold_voxels", dest='threshold_voxels', type=int, default= 5,  help="minumun number of point to create a voxel")
args=parser.parse_args()

def process(input_path, output_path):
    if args.object_type == 'pointcloud':
        pcd = read_point_cloud(input_path)
        points=np.asarray(pcd.points)
        print("Total Points: ", points.shape[0])
        if not args.skip_split:
            path_chunk=split_3D.split_3D(pcd,args.split_x,args.split_y,args.split_z, output_path, args.object_type, args.mode)
        else:
            path_chunk=os.path.join(output_path, "split_ply")
        subprocess.run(["blender", args.path_template, "-b", "--python","preprocess/voxelize_ply.py", "--", "-step_0", 
                    str(args.step_0), "-step_1", str(args.step_1), "-step2", str(args.step_2), "-threshold_voxels", str(args.threshold_voxels),
                    "-input_path", path_chunk, "-output_path", output_path, "-template_path", args.path_template])
    elif args.object_type == 'mesh':
        mesh = read_triangle_mesh(input_path)
        if not args.skip_split:
            path_chunk=split_3D.split_3D(mesh,args.split_x,args.split_y,args.split_z, output_path, args.object_type, args.mode)
        else:
            path_chunk=os.path.join(output_path, "split_ply")
        subprocess.run(["blender", args.path_template, "-b", "--python","preprocess/process_mesh.py", "--", "-input_path", path_chunk, 
                        "-output_path", output_path, "-template_path", args.path_template])

if not os.path.exists(args.output_path):
    os.makedirs(args.output_path)

if os.path.isdir(args.input_path):
    for f in os.listdir(args.input_path):
        if f.endswith(".ply"):
            input_path = os.path.join(args.input_path,f)
            output_path = os.path.join(args.output_path,f.replace(".ply",""))
            if not os.path.exists(output_path):
                os.makedirs(output_path)
            process(input_path,output_path)
else:
    process(args.input_path, args.output_path)