import numpy as np
import time
import bpy
import bmesh
import os
import sys
import argparse

### BLENDER ###

if '--' in sys.argv:
    argv = sys.argv[sys.argv.index('--') + 1:]
    parser = argparse.ArgumentParser(description="Blender Mesh Tool")
    parser.add_argument("-input_path","--input_path", dest='path_input', type=str, default="test/split_ply/", help="where input files are stored")
    parser.add_argument("-output_path","--output_path", dest='path_output', type=str, default="test/", help="where output files will be stored")
    parser.add_argument("-template_path","--template_path", dest='path_template', type=str, default="../resources/template.blend", help="empty blender template file")
    args=parser.parse_known_args(argv)[0]

basename=args.path_output.split("/")[-1].strip()
path_output_rgb = os.path.join(args.path_output,"RGB")
path_output_empty = os.path.join(args.path_output,"Empty")

if not os.path.exists(path_output_rgb):
    os.makedirs(path_output_rgb)
if not os.path.exists(path_output_empty):
    os.makedirs(path_output_empty)

for f in os.listdir(args.path_input):
    filepath=os.path.join(args.path_input,f)
    bpy.ops.import_mesh.ply(filepath=filepath)
    bpy.ops.wm.save_as_mainfile(filepath=os.path.join(path_output_rgb,basename + "_" + f.replace(".ply",".blend")))
    chunk=bpy.data.objects[0]
    bpy.data.objects[0].data.vertex_colors.remove(bpy.data.objects[0].data.vertex_colors[0])
    bpy.data.objects[0].data.vertex_colors.new()
    bpy.ops.wm.save_as_mainfile(filepath=os.path.join(path_output_empty,basename + "_" + f.replace(".ply",".blend")))
    bpy.ops.wm.open_mainfile(filepath=args.path_template)
