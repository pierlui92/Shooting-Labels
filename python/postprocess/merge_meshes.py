from open3d import *
import os
import argparse
from utils.utils import *

def merge_chunks_color(path_mesh, path_chunks, output_path):
    basename = os.path.splitext(os.path.basename(path_mesh))[0]
    main = read_triangle_mesh(path_mesh)
    main.paint_uniform_color([0, 0, 0])
    vertices=np.asarray(main.vertices)
    colors=np.asarray(main.vertex_colors)
    pcd_main = PointCloud()
    pcd_main.points = Vector3dVector(vertices)
    tree = KDTreeFlann(pcd_main)
    for f in os.listdir(path_chunks):
        if basename in f:
            chunk=read_triangle_mesh(os.path.join(path_chunks,f))
            vertices_chunk=np.asarray(chunk.vertices)
            color_chunks=np.asarray(chunk.vertex_colors)

            for c in range(vertices_chunk.shape[0]):
                [k,idx,_] = tree.search_knn_vector_3d(Vector3dVector([vertices_chunk[c]])[0],1)
                colors[idx[0]] = color_chunks[c]

    write_triangle_mesh(output_path, main)
    return main

def merge_chunks_ids(path_mesh, path_chunks, id2color_lut, output_path):
    basename = os.path.splitext(os.path.basename(path_mesh))[0]
    main = read_triangle_mesh(path_mesh)
    main.paint_uniform_color([0, 0, 0])
    vertices=np.asarray(main.vertices)
    colors=np.asarray(main.vertex_colors)
    faces=np.asarray(main.triangles)
    centroids=np.mean(vertices[faces],axis=1)
    ids=[0]*faces.shape[0]
    pcd_main = PointCloud()
    pcd_main.points = Vector3dVector(centroids)
    tree = KDTreeFlann(pcd_main)
    for f in os.listdir(path_chunks):
        if basename in f:
            vertices_chunk, faces_chunk, ids_chunk, colors_chunk = read_ply_with_custom_fields(os.path.join(path_chunks,f), read_colors=False)
            centroids_chunk=np.mean(vertices_chunk[faces_chunk],axis=1)
            for c in range(centroids_chunk.shape[0]):
                [k,idx,_] = tree.search_knn_vector_3d(Vector3dVector([centroids_chunk[c]])[0],1)
                ids[idx[0]]=ids_chunk[c]
                colors[faces[idx[0]]] = id2color_lut[ids_chunk[c]]/255
    write_ply_with_custom_fields(output_path,vertices,colors,faces,ids)
    return main

def merge(path_original_regions, path_chunks, classes, output_path, format='id', visualize =False ):
    if os.path.isdir(path_original_regions):
        if not os.path.exists(output_path):
            os.makedirs(output_path)
        for idx,mesh_path in enumerate(os.listdir(path_original_regions)):
            print(mesh_path, idx)
            if mesh_path.endswith(".ply"):
                if format=='color':
                    mesh_colored = merge_chunks_color(os.path.join(path_original_regions,mesh_path), path_chunks,os.path.join(output_path, mesh_path))
                else:
                    id2color,color2id,id2color_lut,_,_ = decode_classes(classes)
                    mesh_colored = merge_chunks_ids(os.path.join(path_original_regions,mesh_path), path_chunks, id2color_lut, os.path.join(output_path,mesh_path))
            if visualize:
                draw_geometries([mesh_colored])   
    else:
        if format=='color':
            mesh_colored=merge_chunks_color(path_original_regions,path_chunks,output_path)
        else:
            id2color,color2id,id2color_lut,_,_ = decode_classes(classes)
            mesh_colored =merge_chunks_ids(os.path.join(path_original_regions,mesh_path), path_chunks, id2color_lut, output_path)      
        if visualize:
            draw_geometries([mesh_colored])        

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("--path_original_regions", type=str, required=True, help="input ply")
    parser.add_argument("--path_chunks", type=str, required=True, help="folder to labeled chunks")
    parser.add_argument("--output_path", type=str, default="../merge/", help="output_path")
    parser.add_argument("--classes", type=str, default='../../unity/Assets/CSV/ColorConfiguration.csv', help="classes csv")
    parser.add_argument("--format", type=str, default='id', choices=['id','color'], help="merge id chunk or color chunk")
    parser.add_argument("--visualize", action='store_true', help="visualize_results")
    args=parser.parse_args()

    merge(args.path_original_regions, args.path_chunks, args.classes, args.output_path, args.format, args.visualize)
