from open3d import *
import argparse
import os

parser = argparse.ArgumentParser()
parser.add_argument("--input_path", type=str, default='/data/pier_data/3DReconstruct/2011_09_30_drive_0020/splits_20x20/', help="input ply")
parser.add_argument("--output_path", type=str, default='/data/pier_data/3DReconstruct/2011_09_30_drive_0020/splits_20x20_bin', help="output folder")
args=parser.parse_args()

if not os.path.exists(args.output_path):
    os.makedirs(args.output_path)

for f in os.listdir(args.input_path):
    if f.endswith(".ply"):
        pcd=read_point_cloud(os.path.join(args.input_path,f))
        write_point_cloud(os.path.join(args.output_path,f),pcd)