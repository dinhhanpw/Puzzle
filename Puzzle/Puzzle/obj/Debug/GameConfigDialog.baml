   M S B A M L   `   `   `  ����A  =Puzzle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:PuzzlePuzzle  5   	       Puzzle.GameConfigDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:Puzzle     5   	   M IFluent, Version=7.0.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546fFluenturn:fluent-ribbon  5   	   -   5          � Icon$  Icons/ic_setting.png��5
   	    � Title$ Game Config Dialog��5   	   $	��250��6$   $	��500��61   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $
��10 5q�6   $��Image:��6"   $��Right=�60   ]� 5   
   $��1��6   ��. .+�5      �� -   5        $��browerButton��5      $
��10 5q�6)   $��5q�67   $��Right=�6C   $��Broswer��6Z   6l   �� 5      -   5        $��nameImageTextBlock��5   &   $
��20 5q�6   $��Stretch=�6@   5      �� 5   
   $��1��6   $
��10 5q�6!   $��Size:��6/   $��Right=�6<   �� 5   
   $��1��6   $��1��6"    �� Orientation$ 
Horizontal=�62   .+�5      	� -   5        $��
size3Radio��5      $
��10 5q�6,    	� 	GroupName$ 	sizeBoard��6:   $	��3x3��6P   6^   	� 5       -   5        $��
size4Radio��5       $
��10 5q�6,   $ 	sizeBoard��6:   $	��4x4��6P   6^   	� 5!      -   5        $��
size5Radio��5!      $
��10 5q�6,   $ 	sizeBoard��6:   $	��5x5��6P   6^   5"      �� 5#   
   $��2��6   $
��10 5q�6!   $��Level Of Difficult:��6/   �� 5$   
   $��2��6   $��1��6"   $ 
Horizontal=�62   .+�5%      	� -   5        $��easyGameRadio��5%      $��	10,0,10,5q�6/   $ 	difficult��6B   $
��Easy��6X   $��15.96��6�   $��Bottom=�6�   	� 5&      -   5        $��mediumGameRadio��5&      $
��10 5q�61   $ 	difficult��6?   $��Medium��6U   6f   	� 5'      -	   5        $��hardGameRadio��5'      $
��10 5q�6/   $ 	difficult��6=   $
��Hard��6S   6b   5(      ]� 5)   
   $��3��6   $��1��6!   ��.  .+�5*      �� $��15 15 50 15q�6   $��50��6*   $��Right=�65   $��Cancel��6L    �� IsCancel . 6]   �� 5+      -
   5        $��okButton��5+      $
��0 15q�6%   $��50��63   $��Right=�6>   $��OK��6U    �� 	IsDefault . 6s   5,      5/      50      