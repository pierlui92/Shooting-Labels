# export PYTHONPATH=$PYTHONPATH:`pwd`
# player="pier"
# echo $player
# python3 postprocess/miou.py --input_path "/data/pier_data/VRTool/python/matterport_v2/integration_expanded/" --gt "/data/pier_data/VRTool/python/matterport_v2/gt_matterport/gt_eigen13/" --output "results_miou_v2/integration_expanded.txt" --classes ../unity/Assets/CSV/ColorConfiguration.csv

# export PYTHONPATH=$PYTHONPATH:`pwd`
# player="pier"
# echo $player
# python3 postprocess/expand_labels.py --input "/home/pier/ShootingLabels/python/matterport_house_6rooms/results/"$player"/merge/" --output "/home/pier/ShootingLabels/python/matterport_house_6rooms/results/"$player"/merge_expanded/" --classes ../unity/Assets/CSV/ColorConfiguration.csv
# python3 postprocess/miou.py --input_path "/home/pier/ShootingLabels/python/matterport_house_6rooms/results/"$player"/merge_expanded/" --gt "/home/pier/ShootingLabels/python/matterport_house_6rooms/gt_matterport/gt_eigen13/" --output results_miou/$player"_expanded.txt" --classes ../unity/Assets/CSV/ColorConfiguration.csv

# export PYTHONPATH=$PYTHONPATH:`pwd`
# conf=1
# python3 postprocess/expand_labels.py --input "/home/pier/ShootingLabels/python/matterport_house_6rooms/integration/" --output "/home/pier/ShootingLabels/python/matterport_house_6rooms/integration_expanded_"$conf"/" --classes ../unity/Assets/CSV/ColorConfiguration.csv --threshold $conf
# python3 postprocess/miou.py --input_path "/home/pier/ShootingLabels/python/matterport_house_6rooms/integration_expanded_"$conf"/" --gt "/home/pier/ShootingLabels/python/matterport_house_6rooms/gt_matterport/gt_eigen13/" --output "results_miou/integration_expanded_"$conf".txt" --classes ../unity/Assets/CSV/ColorConfiguration.csv

export PYTHONPATH=$PYTHONPATH:`pwd`

python3 postprocess/miou.py --input_path "/data/pier_data/VRTool/python/matterport_v2/integration_expanded_weighted_0.4/" --gt "/data/pier_data/VRTool/python/matterport_v2/gt_matterport/gt_eigen13/" --output "results_miou_v2/integration_expanded_weighted_0.4.txt" --classes ../unity/Assets/CSV/ColorConfiguration.csv

for conf in 0.5 0.55 0.6 0.65 0.8 1
do
 python3 postprocess/expand_labels.py --input "/data/pier_data/VRTool/python/matterport_v2/integration" --output "/data/pier_data/VRTool/python/matterport_v2/integration_expanded_weighted_"$conf"/" --classes ../unity/Assets/CSV/ColorConfiguration.csv --threshold $conf --weight_by_conf

 python3 postprocess/miou.py --input_path "/data/pier_data/VRTool/python/matterport_v2/integration_expanded_weighted_"$conf"/" --gt "/data/pier_data/VRTool/python/matterport_v2/gt_matterport/gt_eigen13/" --output "results_miou_v2/integration_expanded_weighted_"$conf".txt" --classes ../unity/Assets/CSV/ColorConfiguration.csv
done
 
# for i in 0.4 0.6 0.8 1
# do
# python3 postprocess/miou.py --input_path "/data/pier_data/VRTool/python/matterport_v2/integration_freq" --gt "/data/pier_data/VRTool/python/matterport_v2/gt_matterport/gt_eigen13" --output "results_miou_v2/integration_freq_"$i".txt" --classes ../unity/Assets/CSV/ColorConfiguration.csv --th $i
# done