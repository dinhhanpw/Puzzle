   M S B A M L   `   `   `  ���� A  =Puzzle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:PuzzlePuzzle      Puzzle.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     M IFluent, Version=7.0.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546fFluenturn:fluent-ribbon   localclr-namespace:Puzzle  -     � WindowState$  	Maximized=� � 
ResizeMode$ 	CanResize=� � Title$ 
MainWindow�� � Icon$ Icons/ic_puzzle.png��
����d�.���  � ColumnDefinitions �� $��2* ��� $��* � � RowDefinitions �� $
��Auto ��� $��* ��� $��* �.+�  Fluent.Ribbon  $��2��   Tabs.   Fluent.RibbonTabItem     Header$
 Game��
����d�   Groups.   Fluent.RibbonGroupBox  $
��File��.@�  Fluent.Button  -   $��newGameButton��	   Header$		 New��
   	LargeIcon$
 Icons/ic_newgame.png��$	��2 0q� �� BitmapScalingMode$ HighQuality=�  -   $��saveGameButton��$
	 Save��$
 Icons/ic_save.png��$	��2 0q�$ HighQuality=�  -   $��loadGameButton��$
	 Load��$
 Icons/ic_load.png��$	��2 0q�$ HighQuality=�  $
��play��.@�  -   $��
playButton��$
	 Play��$
 Icons/ic_play.png��$	��2 0q�$ HighQuality=�  -   $��pauseuButton��$	 Pause��$
 Icons/ic_pause.png��$	��2 0q�$ HighQuality=�  -   $��
stopButton��$
	 Stop��$
 Icons/ic_stop.png��$	��2 0q�$ HighQuality=�  .@�  -   $��
undoButton��$
	 Undo��$
 Icons/ic_undo.png��$	��2 0q�$ HighQuality=�  -	   $��
redoButton��$
	 Redo��$
 Icons/ic_redo.png��$	��2 0q�$ HighQuality=�  $ Options��.   $��Music��.@�  Fluent.ToggleButton  -
   $��toggleMusicButton��   	LargeIcon$ Icons/ic_music.png��$	��5 0q�$ HighQuality=�  Fluent.RadioButton  -    	� 	GroupName$ music��   Header$ Music 1�� d� 	IsChecked$
 TrueY�  -   $ music��$ Music 2��  -   $ music��$ Music 3��  .@�  -   $	 
High Score��$
 Icons/ic_highscore.png���� -   $��canvas��$��1��$��2���� $��5q�$��1��$��1��
�������$��5q�._��� $��5q�.+��� -   $��sampleImage��$ HighQuality=��� $��5q�$��2��$��1��
�������$��5q�._�]� $��Stretch=�$��Stretch=�.+��� $	��Top=�$��Center=�$
��0 10q�$��36(�$
��Time��$��
UltraBlack"��� -   $��stopWatchTextBlock��$��Consolas)�$��46(�$��Center=�$��Center=�$��00:00��