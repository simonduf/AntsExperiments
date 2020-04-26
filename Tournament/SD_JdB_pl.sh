toolsPath=$PWD/../tools

bot1Path="mono $PWD/Contenders/Bot_SD/Bot_SD1.exe"
bot2Path="mono $PWD/Contenders/Bot_JdB/Bot_JdB.exe"
bot3Path="mono $PWD/Contenders/Bot_pl/Bot_pl.exe"
Map=$toolsPath/maps/maze/maze_p03_01.map

echo $bot1Path
echo $Map

python "$toolsPath/playgame.py" $preViz --engine_seed 42 --player_seed 42 --end_wait=11.25 --verbose --log_dir game_logs --turns 1000 --map_file $Map "$bot1Path" "$bot2Path" "$bot3Path" $postViz
