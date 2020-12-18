from open3d import *
import argparse
import numpy as np
import os
import time

def split_3D(data,sx,sy,sz,output_path,type3D='pointcloud',mode='number'):
    ### SPLIT ###
    if type3D=='pointcloud':
        crop = crop_point_cloud
        write = write_point_cloud
    elif type3D == 'mesh':
        crop = crop_triangle_mesh
        write = write_triangle_mesh

    if mode=='number':
        splits=np.asarray([sx,sy,sz])
        minArray = data.get_min_bound()
        maxArray = data.get_max_bound()
        dimension_split = np.abs(maxArray-minArray)/splits
    elif mode=='dimension':
        minArray = data.get_min_bound()
        maxArray = data.get_max_bound()
        dimension_split=np.asarray([sx,sy,sz])
        dimension_split=np.where(dimension_split<0, maxArray-minArray, dimension_split)
        splits = np.ceil(np.abs(maxArray-minArray)/dimension_split).astype(int)
    
    fout=open(os.path.join(output_path,"boundaries.txt"),"w")
    fout.write("LOG:" + "\n")
    fout.write("NUM_SPLITS_XYZ " + str(splits[0]) + " " + str(splits[1]) + " " + str(splits[2]) + "\n")
    fout.write("MIN_BOUND " + np.array2string(minArray) + "\n")
    fout.write("MAX_BOUND " + np.array2string(maxArray) + "\n")
    fout.write("SPLIT_DIMENSION " + np.array2string(dimension_split) +"\n")

    if not os.path.exists(os.path.join(output_path,"split_ply")):
        os.makedirs(os.path.join(output_path,"split_ply"))

    for i in range(splits[0]):
        for j in range(splits[1]):
            for k in range(splits[2]):
                start = minArray + np.asarray([i,j,k])*dimension_split
                end = minArray + np.asarray([i+1,j+1,k+1])*dimension_split
                fout.write(str(i) + " " +str(j) + " " +str(k) + " " + str(start[0]) + " " + str(start[1]) + " " + str(start[2]) + " " + str(end[0]) + " " + str(end[1]) + " " + str(end[2]) + "\n")  
                tmp=crop(data, start, end)
                write(os.path.join(output_path, "split_ply", "chunk_" + str(i) + "_" + str(j) + "_" + str(k) + ".ply"), tmp, write_ascii=True)

    fout.close()

    return os.path.join(output_path, "split_ply")