import numpy as np
from open3d import *

def save_view_point(pcd, filename):
    vis = Visualizer()
    vis.create_window()
    vis.add_geometry(pcd)
    vis.run() # user changes the view and press "q" to terminate
    param = vis.get_view_control().convert_to_pinhole_camera_parameters()
    write_pinhole_camera_parameters(filename, param)
    vis.destroy_window()


def load_view_point(pcd, filename, pose):
    vis = Visualizer()
    vis.create_window()
    ctr = vis.get_view_control()
    param = read_pinhole_camera_parameters(filename)
    param.extrinsic=pose
    vis.add_geometry(pcd)
    ctr.convert_from_pinhole_camera_parameters(param)
    vis.run()
    vis.destroy_window()

def decode_pose(line):
    splits = line.split(" ")
    pose=np.zeros([4,4])
    pose[3,3]=1
    for idx,s in enumerate(splits):
        pose[idx//4, idx%4]=float(s)
    return pose

pose_path="/home/pier/pier_data/Kitti/KittiODOMETRY/poses/2011_09_30_drive_0020_sync.txt"
poses=open(pose_path).readlines()
pose=decode_pose(poses[750])
pose=np.linalg.inv(pose)

#pcd = PointCloud()
pcd=read_point_cloud("/data/pier_data/3DReconstruct/2011_09_30_drive_0020/point_cloud_labeled.ply")
load_view_point(pcd, "params.json", pose)

