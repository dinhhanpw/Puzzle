   M S B A M L   `   `   `  ���� A  =Puzzle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:PuzzlePuzzle      Puzzle.GameConfigDialog   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:Puzzle     M IFluent, Version=7.0.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546fFluenturn:fluent-ribbon  -     � Icon$  Icons/ic_setting.png�� � Title$ Game Config Dialog��$	��250��$	��500��.���  � ColumnDefinitions �� $
��Auto ��� $��* � � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $
��10 5q�$��Image:��$��Right=�]� $��1����. .+��� -   $��browerButton��$
��10 5q�$��5q�$��Right=�$��Broswer���� -   $��nameImageTextBlock��$
��20 5q�$��Stretch=��� $��1��$
��10 5q�$��Size:��$��Right=��� $��1��$��1�� �� Orientation$ 
Horizontal=�.+�	� -   $��
size3Radio��$
��10 5q� 	� 	GroupName$ 	sizeBoard��$	��3x3��	� -   $��
size4Radio��$
��10 5q�$ 	sizeBoard��$	��4x4��	� -   $��
size5Radio��$
��10 5q�$ 	sizeBoard��$	��5x5���� $��2��$
��10 5q�$��Level Of Difficult:���� $��2��$��1��$ 
Horizontal=�.+�	� -   $��easyGameRadio��$��	10,0,10,5q�$ 	difficult��$
��Easy��$��15.96��$��Bottom=�	� -   $��mediumGameRadio��$
��10 5q�$ 	difficult��$��Medium��	� -	   $��hardGameRadio��$
��10 5q�$ 	difficult��$
��Hard��]� $��3��$��1����.  .+��� $��15 15 50 15q�$��50��$��Right=�$��Cancel�� �� IsCancel . �� -
   $��okButton��$
��0 15q�$��50��$��Right=�$��OK�� �� 	IsDefault . 