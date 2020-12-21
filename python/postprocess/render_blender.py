import bpy
import os
import numpy as np

path_semantic="/data/pier_data/3DReconstruct/2011_09_30_drive_0020/blender_splits_20x20_LOD0"
path_intrinsics="/home/pier/pier_data/Kitti/KittiODOMETRY/camera_params/2011_09_30_drive_0020_sync/calib.txt"
path_extrinsics="/home/pier/pier_data/Kitti/KittiODOMETRY/poses/2011_09_30_drive_0020_sync.txt"
output_path="/data/pier_data/3DReconstruct/2011_09_30_drive_0020/renders"

res_x=1226
res_y=370

intrinsics=open(path_intrinsics).readline()
_,f_x,_,c_x,_,_,f_y,c_y,_,_,_,_,_ = intrinsics.split(" ")
f_x=float(f_x)
f_y=float(f_y)
c_x=float(c_x)
c_y=float(c_y)

if not os.path.exists(output_path):
    os.makedirs(output_path)

def decode_pose(line):
    splits = line.split(" ")
    pose=np.zeros([4,4])
    pose[3,3]=1
    for idx,s in enumerate(splits):
        pose[idx//4, idx%4]=float(s)
    return pose

def setRenderParams():
    bpy.data.scenes["Scene"].render.resolution_x = res_x
    bpy.data.scenes["Scene"].render.resolution_y = res_y
    bpy.data.scenes["Scene"].render.image_settings.color_mode='RGB'
    bpy.data.scenes["Scene"].render.tile_x = 512
    bpy.data.scenes["Scene"].render.tile_y = 512
    bpy.data.scenes["Scene"].cycles.device = 'GPU'
    bpy.data.scenes["Scene"].render.resolution_percentage=100
    bpy.data.scenes["Scene"].render.use_shadows=False
    bpy.data.scenes["Scene"].render.use_sss=False
    bpy.data.scenes["Scene"].render.use_envmaps =False
    bpy.data.scenes["Scene"].render.use_raytrace=False
    bpy.data.scenes["Scene"].render.use_world_space_shading=False
    bpy.data.scenes["Scene"].render.use_antialiasing = False
    bpy.data.worlds["World"].horizon_color=(0,0,0)

def setCameraParams(cam):
    pixel2mm = cam.sensor_width/res_x
    cam.type = 'PERSP'
    cam.lens = f_x*pixel2mm
    cam.lens_unit = 'MILLIMETERS'
    cam.shift_x = (res_x/2 - c_x)/res_x
    cam.shift_y = (res_y/2 - c_y)/res_y
    cam.clip_start = 0.01
    cam.clip_end = 5000

if "Camera" not in bpy.data.objects.keys():
    print("Adding Camera")
    bpy.ops.object.camera_add()

poses = open(path_extrinsics).readlines()

blender_pose=([[1,0,0,0],
            [0,-1,0,0],
            [0,0,-1,0],
            [0,0,0,1]])

cam_obj=bpy.data.objects["Camera"]
cam=bpy.data.cameras["Camera"]
bpy.data.scenes["Scene"].camera=cam_obj
setCameraParams(cam)
setRenderParams()

for idx,pose in enumerate(poses):
    mat=decode_pose(pose)
    mat=np.transpose(mat,axes=[1,0])
    cam_obj.matrix_world=np.dot(blender_pose,mat)
    bpy.data.scenes["Scene"].render.filepath = os.path.join(output_path,"{:010d}".format(idx))
    bpy.ops.render.render(write_still=True)