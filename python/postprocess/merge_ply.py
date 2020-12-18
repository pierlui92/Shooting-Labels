import os
import bpy
from io_mesh_ply import import_ply

path_chunks="/data/pier_data/3DReconstruct/2011_09_30_drive_0020/labels"
path_bounds="/data/pier_data/3DReconstruct/2011_09_30_drive_0020/splits_20x20/boundaries.txt"
path_coords="/data/pier_data/3DReconstruct/2011_09_30_drive_0020/splits_20x20/coordinate.txt"
path_output="/data/pier_data/3DReconstruct/2011_09_30_drive_0020/blender_labels"

if not os.path.exists(path_output):
    os.makedirs(path_output)

boundaries=open(path_bounds).readlines()
coords=open(path_coords).readlines()

coord_dict={}
for l in coords:
    x,y,z, coord_x,coord_y,coord_z=l.strip().split(" ")
    coord_dict[x+" " +y+" "+z]=[float(coord_x),float(coord_y),float(coord_z)]

_,num_splits_x,num_splits_y,num_splits_z=boundaries[1].split(" ")
num_splits_x=int(num_splits_x)
num_splits_y=int(num_splits_y)
num_splits_z=int(num_splits_z)

for f in os.listdir(path_chunks):
    import_ply.load_ply(os.path.join(path_chunks, f))

material = bpy.data.materials.new('Shadeless')
material.use_shadeless = True
material.use_vertex_color_paint=True

for ob_name in bpy.data.objects.keys():
    if "chunk" in ob_name:
        print(ob_name)
        _,x,y,z=ob_name.strip().split("_")
        coord=coord_dict[x+" " +y+" "+z]
        bpy.data.objects[ob_name].location=(coord[0],coord[1],coord[2])
        bpy.data.objects[ob_name].data.materials.append(material)

bpy.ops.wm.save_as_mainfile(filepath=path_output)