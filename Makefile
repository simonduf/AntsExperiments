define custom_help_text
Building your Bot:
Each bot's source code is contained inside of their own
folders. Each bot creator is responsible for it's own 
makefile which, in turn, is responsible for the creation
of an executable which should, when stable and ready for
competition, be added to the ReleasedContenders folder
for others to easily compete against.

Running a Match:
Here are some examples of the syntax you should use to get
bots to compete with one another:
make 2ffa bot1=jon bot2=simon
make 3ffa bot1=pl default_bot=hunter
endef

#######################################################################
#  List your bots here so that they can be easily competed against    #
#######################################################################
bot_lefty_description := Super easy to beat bot that only goes left
bot_lefty_release_Windows_NT = python $(tools_path)/sample_bots/python/LeftyBot.py

bot_hunter_description := Super easy to beat bot that is overy aggresive and not very good at pathfinding
bot_hunter_release_Windows_NT = python $(tools_path)/sample_bots/python/HunterBot.py

bot_jon_description := Jonathan's bot
bot_jon_devcmd := echo TEST #Intentionally Left Blank, this would be used to call build script
bot_jon_devbuild := Bot_JdB/bin/Debug/Bot_JdB.exe
bot_jon_release_Windows_NT := ReleasedContenders/Bot_JdB/Bot_JdB.exe

bot_simon_description := Simon's bot
bot_simon_release_Windows_NT := ReleasedContenders/Bot_SD/Bot_SD1.exe

bot_pl_description := Pierre-Luc's bot
bot_pl_makefile := Bot_SD
bot_pl_release_Windows_NT := ReleasedContenders/Bot_pl/Bot_pl.exe
######################################################################


bot1exec = $(bot_$(strip $(bot1))_release_$(OS))
bot2exec = $(bot_$(strip $(bot2))_release_$(OS))
bot3exec = $(bot_$(strip $(bot3))_release_$(OS))
bot4exec = $(bot_$(strip $(bot4))_release_$(OS))

log_directory := CoreFramework/game_logs
tools_path := CoreFramework/tools
play_game_script := $(tools_path)/playgame.py
map_2_players := $(tools_path)/maps/maze/maze_p02_01.map
map_3_players := $(tools_path)/maps/maze/maze_p03_01.map
map_4_players := $(tools_path)/maps/maze/maze_p04_11.map

default_bot := lefty		# !h: The default bot for unspecified bot slots
bot1 := $(default_bot)		# !h: The bot to use for slot 1
bot2 := $(default_bot)		# !h: The bot to use for slot 2
bot3 := $(default_bot)		# !h: The bot to use for slot 3
bot4 := $(default_bot)		# !h: The bot to use for slot 4
turns := 500				# !h: The maximum number of turns to compute
end_wait := 11.25			# !h: The time the program will wait for bots to finish logging
devbot1 :=					# !h: Use dev version for bot 1, great for testing

define play
	python "$(play_game_script)" 				\
		--engine_seed 42 						\
		--player_seed 42 						\
		--end_wait=$(strip $(end_wait))			\
		--verbose 								\
		--turns $(strip $(turns))				\
		--log_dir $(strip $(log_directory))		\
		--map_file "$(strip $1)" 				\
		$(call bot_arg, $2)						\
		$(call bot_arg, $3)						\
		$(call bot_arg, $4)						\
		$(call bot_arg, $5)						\
		$(call bot_arg, $6)						\
		$(call bot_arg, $7)						\
		$(call bot_arg, $8)						\
		$(call bot_arg, $9)						
endef

define bot_arg
	$(if $(strip $1), "$(strip $1)")
endef

define check_bot
	$(eval bot_id := $(strip $1))
	$(eval bot_name := $(strip $($(bot_id))))
	$(eval bot_exec := $(strip $($(bot_id)exec)))
	$(if $(bot_exec),,$(error Failed to find bot: $(bot_name)))
	$(info $(bot_id) path: $(bot_exec))
endef

define replace_dev_bot
	$(if $(strip $(devbot1)), $(call __replace_dev_bot))
endef

define __replace_dev_bot
	$(info Replacing bot 1 with development bot)
	@$(bot_$(devbot1)_devcmd)
	$(eval bot1exec := $(bot_$(devbot1)_devbuild))
endef


.PHONY: 2ffa # Make two bots compete with one another
2ffa:
	$(call replace_dev_bot)
	$(call check_bot, bot1)
	$(call check_bot, bot2)
	$(call play, $(map_2_players), $(bot1exec), $(bot2exec))

.PHONY: 3ffa # Make three bots compete with one another
3ffa:
	$(call replace_dev_bot)
	$(call check_bot, bot1)
	$(call check_bot, bot2)
	$(call check_bot, bot3)
	$(call play, $(map_3_players), $(bot1exec), $(bot2exec), $(bot3exec))

.PHONY: 4ffa # Make four bots compete with one another
4ffa:	
	$(call replace_dev_bot)
	$(call check_bot, bot1)
	$(call check_bot, bot2)
	$(call check_bot, bot3)
	$(call check_bot, bot4)
	$(call play, $(map_4_players), $(bot1exec), $(bot2exec), $(bot3exec), $(bot4exec))

.PHONY help:
help: # Generate list of targets with descriptions
	$(info $(custom_help_text))	
	@printf "\n\nBuild Commands:\n"
	@grep '^.PHONY: .*' Makefile | sed -r 's/^.PHONY:\s*(\S*)\s*#\s*(.*)/\1\t\2/' | expand -t10
	@printf "\nList of Bots:\n"
	@grep '^bot_\S\+_description.*' Makefile | sed -r 's/^bot_(.+)_description.*:=(.*)$$/\1\t\2/' | expand -t10
	@printf "\nOptions:\n"
	@printf "Name\tDefault\tDescription\n" | expand -t20
	@grep '^\S*\s\?:=.*!h.*$$' Makefile | sed -r 's/^(\S*)\s*:=\s*(\S*).*#\s*!h:\s*(.*)/\1\t\2\t\3/' | expand -t20
