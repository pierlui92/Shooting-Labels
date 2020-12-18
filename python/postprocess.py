import argparse
import os
from postprocess.merge_meshes import merge

parser = argparse.ArgumentParser()
parser.add_argument("--input_path", type=str, default='/data/pier_data/VRTool/python/matterport_house_6rooms/testing/ids/', help="input folder")
parser.add_argument("--original_path", type=str, default='/data/pier_data/VRTool/python/matterport_house_6rooms/gt_matterport/gt_original/', help="original path")
parser.add_argument("--output_path", type=str, default='/data/pier_data/VRTool/python/matterport_house_6rooms/testing', help="")
parser.add_argument("--classes", type=str, default='../unity/Assets/CSV/ColorConfiguration.csv', help="classes csv")
parser.add_argument("--object_type", type=str, default='mesh',choices=['mesh','pointcloud'], help="mesh or pointcloud")
parser.add_argument("--integrate", action='store_true', help="")
parser.add_argument("--expand_labels", action='store_true', help="")
parser.add_argument("--evaluate", action='store_true', help="")
args=parser.parse_args()

for player in os.listdir(args.input_path):
    if not os.path.exists(os.path.join(args.output_path,player,"merge")):
        merge(args.original_path, os.path.join(args.input_path,player,"chunks"), args.classes, os.path.join(args.output_path,player,"merge"))
        
## ENTROPY
## INTEGRATION ALL RESULTS##
### ESPAND LABELS ###
## EVALUATE