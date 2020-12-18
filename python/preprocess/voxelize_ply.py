import numpy as np
import time
import bpy
import bmesh
import os
import sys
import argparse
from mathutils import Vector

if '--' in sys.argv:
    argv = sys.argv[sys.argv.index('--') + 1:]
    parser = argparse.ArgumentParser(description="Blender Voxelize Ply")
    parser.add_argument("-step_0","--step_0", dest='step_0', type=float, default= 0.25,  help="discretization step LOD 0")
    parser.add_argument("-step_1","--step_1", dest='step_1', type=float, default= 0.5,  help="discretization step LOD 1")
    parser.add_argument("-step_2","--step_2", dest='step_2', type=float, default= 1.0,  help="discretization step LOD 2")
    parser.add_argument("-threshold_voxels","--threshold_voxels", dest='threshold_voxels', type=int, default= 5,  help="minumun number of point to create a voxel")
    parser.add_argument("-input_path","--input_path", dest='path_chunk', type=str, default="test/split_ply/", help="where input files are stored")
    parser.add_argument("-output_path","--output_path", dest='path_output', type=str, default="test/", help="where output files will be stored")
    parser.add_argument("-template_path","--template_path", dest='path_template', type=str, default="../resources/template.blend", help="empty blender template file")
    args=parser.parse_known_args(argv)[0]

step_0 = args.step_0
step_1 = args.step_1
step_2 = args.step_2
threshold_voxels = args.threshold_voxels
path_chunk = args.path_chunk
path_output = args.path_output
path_template = args.path_template

def voxelize_chunks(path_chunk, output_path, step=0.1, th=5, LOD=0):
    path_template="../resources/template.blend"
    path_output=os.path.join(output_path,"blender_LOD_" + str(LOD))

    if not os.path.exists(path_output):
        os.makedirs(path_output)

    boundaries=open(os.path.join(output_path,"boundaries.txt")).readlines()
    n_split_x,n_split_y,n_split_z=boundaries[1].split(" ")[1:]
    n_split_x=int(n_split_x)
    n_split_y=int(n_split_y)
    n_split_z=int(n_split_z)

    print(n_split_x,n_split_y,n_split_z)
    
    coords=open(os.path.join(output_path,"coords_chunk.txt"),"w")
    
    for a in range(n_split_x):
        for u in range(n_split_y):
            for c in range(n_split_z):
                id_chunk = [a,u,c]
                print("Chunk: ",a,u,c)
                path_output_chunk=os.path.join(path_output,"chunk_" + str(id_chunk[0]) + "_" + str(id_chunk[1]) + "_" +str(id_chunk[2]) + ".blend")
                path_input_chunk = os.path.join(path_chunk, "chunk_" + str(id_chunk[0]) + "_" + str(id_chunk[1]) + "_" + str(id_chunk[2]) + ".ply")

                if os.path.exists(path_input_chunk):
                    start= time.time()
                    pcd=[]

                    bounds = boundaries[id_chunk[0]*n_split_y*n_split_z + id_chunk[1]*n_split_z + id_chunk[2] + 5].strip().split(" ")[3:]
                    minArray=np.asarray([float(bounds[0]),float(bounds[1]),float(bounds[2])])
                    maxArray=np.asarray([float(bounds[3]),float(bounds[4]),float(bounds[5])])

                    pointCloudLines = open(path_input_chunk).readlines()[11:]
                    for l in pointCloudLines:
                        x,y,z,r,g,b=l.split(" ")
                        x=float(x)
                        y=float(y)
                        z=float(z.strip())
                        pcd.append([x,y,z])
                    vertices = np.asarray(pcd)
                    print("points: ", vertices.shape, "pointCloud read in : ", time.time()-start)

                    maxArrayInt = (maxArray-minArray)/step 
                    maxArrayInt = maxArrayInt.astype(int) + 2
                    #Shift every points
                    voxelsIdx = (vertices - minArray)/step
                    voxelsIdx = voxelsIdx.astype(int)
                    #print("Grid Range: ", maxArrayInt)

                    #Create matrix for discretization
                    grid = np.zeros([maxArrayInt[0], maxArrayInt[1], maxArrayInt[2]])
                    for v in voxelsIdx:
                        grid[v[0],v[1],v[2]] += 1

                    id=0
                    for i in range(grid.shape[0]):
                        for j in range(grid.shape[1]):
                            for k in range(grid.shape[2]):
                                if grid[i,j,k] > th:
                                    coord=np.asarray([i,j,k])*step+minArray
                                    #print("Creating cube ",id)
                                    bpyscene = bpy.data.scenes["Scene"]
                                    # Create an empty mesh and the object.
                                    mesh = bpy.data.meshes.new('Voxel_'+str(id))
                                    basic_cube = bpy.data.objects.new('Voxel_'+str(id), mesh)
                                    basic_cube.location=coord
                                    # Add the object into the scene.
                                    bpyscene.objects.link(basic_cube)
                                    bpyscene.objects.active = basic_cube
                                    basic_cube.select = True
                                    # Construct the bmesh cube and assign it to the blender mesh.
                                    bm = bmesh.new()
                                    bmesh.ops.create_cube(bm, size=step - 0.001)
                                    bm.to_mesh(mesh)
                                    bm.free()
                                    id+=1
                    
                    if id > 0:           
                        print("\nJoining voxels")	
                        bpy.data.scenes["Scene"].objects.active=bpy.data.objects["Voxel_0"]
                        bpy.data.scenes["Scene"].cursor_location = Vector((0.0,0.0,0.0))
                        bpy.ops.object.join()
                        bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
                        coords.write(str(a) + " " + str(u) + " " + str(c) + " " + str(bpy.data.objects["Voxel_0"].location[0]) + " " + 
                                    str(bpy.data.objects["Voxel_0"].location[1]) + " " + str(bpy.data.objects["Voxel_0"].location[2]) + "\n")
                        #Add vertex colors	
                        for obj in bpy.context.scene.objects:
                            obj.data.vertex_colors.new()
                        bpy.ops.wm.save_as_mainfile(filepath=path_output_chunk)
                    
                    print("Time needed: ", time.time()-start)
                    bpy.ops.wm.open_mainfile(filepath=path_template)
    return path_output

def append_lods(path_lod0, path_lod1, path_lod2, path_template, path_output):
    path_output = os.path.join(path_output,"LODS")
    if not os.path.exists(path_output):
        os.makedirs(path_output)

    for f in os.listdir(path_lod0):
        if f.endswith(".blend"):
            blend_path_lod0=os.path.join(path_lod0,f + "/Object")
            blend_path_lod1=os.path.join(path_lod1,f + "/Object")
            blend_path_lod2=os.path.join(path_lod2,f + "/Object")

            bpy.data.scenes["Scene"].layers[0]=True
            bpy.ops.wm.append(directory=blend_path_lod0, link=False, filename="Voxel_0")
            bpy.data.objects["Voxel_0"].name="map_LOD0"
            bpy.data.scenes["Scene"].layers[0]=False
            bpy.data.scenes["Scene"].layers[1]=True
            bpy.ops.wm.append(directory=blend_path_lod1, link=False, filename="Voxel_0")
            bpy.data.objects["Voxel_0"].name="map_LOD1"
            bpy.data.scenes["Scene"].layers[1]=False
            bpy.data.scenes["Scene"].layers[2]=True
            bpy.ops.wm.append(directory=blend_path_lod2, link=False, filename="Voxel_0")
            bpy.data.objects["Voxel_0"].name="map_LOD2"
            bpy.data.scenes["Scene"].layers[2]=False
            
            bpy.ops.wm.save_as_mainfile(filepath=os.path.join(path_output,f))
            bpy.ops.wm.open_mainfile(filepath=path_template)

path_lod0 = voxelize_chunks(path_chunk,path_output, step_0,threshold_voxels,0)
path_lod1 = voxelize_chunks(path_chunk,path_output, step_1,threshold_voxels,1)
path_lod2 = voxelize_chunks(path_chunk,path_output, step_2,threshold_voxels,2)
append_lods(path_lod0,path_lod1,path_lod2, path_template, path_output)
