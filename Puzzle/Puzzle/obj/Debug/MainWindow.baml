   M S B A M L   `   `   `  ����A  =Puzzle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:PuzzlePuzzle  5   	       Puzzle.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   M IFluent, Version=7.0.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546fFluenturn:fluent-ribbon  5   	    localclr-namespace:Puzzle  5   	   -   5          � WindowState$  	Maximized=�5	   	    � 
ResizeMode$ 	CanResize=�5
   	    � Title$ 
MainWindow��5   	    � Icon$ Icons/ic_puzzle.png��5   	   
����d�.��5      �  � ColumnDefinitions 5   
   �� 5      $��2* �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $��* �6   .+�5   
     Fluent.Ribbon  $��2��6      Tabs. 5        Fluent.RibbonTabItem     Header$
 Game��6#   
����d�   Groups. 5        Fluent.RibbonGroupBox  $
��File��6(   .@�5         Fluent.Button  -   5        $��newGameButton��5    $   	   Header$		 New��69   
   	LargeIcon$
 Icons/ic_newgame.png��6F   $	��2 0q�5!   $    �� BitmapScalingMode$ HighQuality=�61   6_     5"      -   5        $��saveGameButton��5"   $   $
	 Save��6:   $
 Icons/ic_save.png��6H   $	��2 0q�5#   $   $ HighQuality=�61   6_     5$      -   5        $��loadGameButton��5$   $   $
	 Load��6:   $
 Icons/ic_load.png��6H   $	��2 0q�5%   $   $ HighQuality=�61   6_   5&        5'      $
��play��6(   .@�5(        -   5        $��
playButton��5(   $   $
	 Play��66   $
 Icons/ic_play.png��6D   $	��2 0q�5)   $   $ HighQuality=�61   5*   $     5+      -   5        $��pauseuButton��5+   $   $	 Pause��68   $
 Icons/ic_pause.png��6G   $	��2 0q�5,   $   $ HighQuality=�61   5-   $     5.      -   5        $��
stopButton��5.   $   $
	 Stop��66   $
 Icons/ic_stop.png��6D   $	��2 0q�5/   $   $ HighQuality=�61   50   $   51        53      .@�54        -   5        $��
undoButton��54   $   $
	 Undo��66   $
 Icons/ic_undo.png��6D   $	��2 0q�55   $   $ HighQuality=�61   6_     56      -	   5        $��
redoButton��56   $   $
	 Redo��66   $
 Icons/ic_redo.png��6D   $	��2 0q�57   $   $ HighQuality=�61   6_   58      59        5;      $ Options��6#   . 5=        $��Music��6(   .@�5>        Fluent.ToggleButton  -
   5        $��toggleMusicButton��5>   *      	LargeIcon$ Icons/ic_music.png��6C   $	��5 0q�5?   *   $ HighQuality=�68   5A   *     Fluent.RadioButton  5C      -   5         	� 	GroupName$ music��5C   )      Header$ Music 1��6;    d� 	IsChecked$
 TrueY�5D   )   6:     5E      -   5        $ music��5E   )   $ Music 2��6;   5F   )     5G      -   5        $ music��5G   )   $ Music 3��6;   5H   )   5I        5J      .@�5K        -   5        $	 
High Score��5K   $   $
 Icons/ic_highscore.png��68   5L   $   5M      5N      5P      �� 5Q   
   -   5        $��canvas��5Q   /   $��1��6   $��2��6   5T      �� 5U   
   $��5q�6   $��1��6   $��1��6,   
�������$��5q�6Q   ._�5V      �� $��5q�6   .+�5W      �� -   5        $��sampleImage��5W      $ HighQuality=�6+   5X      5Y      �� 5Z   
   $��5q�6   $��2��6   $��1��6)   
�������$��5q�6Q   ._�5[      ]� $��Stretch=�6   $��Stretch=�66   .+�5\      �� $	��Top=�6   $��Center=�61   $
��0 10q�6N   $��36(�6\   $
��Time��6j   $��
UltraBlack"�5]      �� 5^      -   5        $��stopWatchTextBlock��5^   2   $��Consolas)�6   $��46(�6L   $��Center=�5_      $��Center=�69   $��00:00��6T   5`      5b      5c      5d      