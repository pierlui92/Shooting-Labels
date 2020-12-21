path="/ShootingLabelsNew/unity/Assets/Resources/Prefabs/RGBMeshes"
path_out="/ShootingLabelsNew\unity/Assets/Resources/Prefabs/RGBMeshes"
mkdir $path_out
cd $path
for file in *.ply
do 
 meshlabserver -i "$file" -o $path_out"${file%.*}.off" -om vc
done