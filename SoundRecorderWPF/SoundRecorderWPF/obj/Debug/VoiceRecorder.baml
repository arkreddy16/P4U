   M S B A M L   `   `   `  ����K  GSoundRecorderWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:SoundRecorderWPFSoundRecorderWPF  5   	   $    SoundRecorderWPF.VoiceRecorder   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:SoundRecorderWPF     5   	     � Title$  Perforce Made Easy��5   	   $��332.147��6$   $��405.931��65   ���	#FFE96049 � Icon$ Images/IconMain.ico��6\    � 
ResizeMode$ NoResize=�6w   $
��Bold"�6�    � WindowStyle$ ThreeDBorderWindow=�6�   .��5	      � .+�5
   
   �� -   5         ��Name$ 	btnSearch��5
      $
��Left=�6$   $��
296,25,0,0q�6?   $	��Top=�6S   
�������$��82��6~   $��26��6�   .��5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� $��Images\Search.png��6   $��20��63   $��20��6>   �� 5      $��Search��6   $��Center=�6*   5      5      �� 5   
   -   5        $ 	btnRecord��5      $
��Left=�6$   $��
296,56,0,0q�6?   $	��Top=�6S   
�������$��82��6   $��26��6�   .��5      �� $ 
Horizontal=�6   .+�5      �� $��Images\record.png��6   $��20��63   $��20��6>   �� 5      -   5         ��Name$ lblbtnRecord��5   *   $��Record��6   $��Center=�6A   5      5      �� 5   
   -   5        $��lblClockTime��5      $��60(�6$   $��Center=�62   $��Center=�6O   �� 5   
   -   5         ��Name$ txtSearchInput��5      $
��Left=�6*   $��45��6E   $��	39,37,0,0q�6Q    �� TextWrapping$
 Wrap=�6d   $�� ��6x   $	��Top=�6�   $	��236��6�   $
��Bold"�6�   �� 5   
   -   5        $ txtSearchResults��5      $
��Left=�6,   $	��156��6G   $��
39,111,0,0q�6T   $
 Wrap=�6h   $	��Top=�6|   $	��332��6�   $��Italic$�6�   $
��Bold"�6�   �� 5   
   -   5        	 ��Name$	 label��5      $��Search Input :-��6   $
��Left=�69   $��	39,10,0,0q�6T   $	��Top=�6g   $��99��6   $��27��6�   $
��Bold"�6�   ���	#FFF5F3F36�   �� 5   
   -   5        $	 
label_Copy��5      $��Search Results :-��6$   $
��Left=�6@   $��	39,83,0,0q�6[   $	��Top=�6n   $	��135��6�   $��27��6�   $
��Bold"�6�   ���	#FFF6F5F56�   �� 5   
   -	   5        $	 	lblStatus��5      $�� ��6#   $
��Left=�6.   $��
39,267,0,0q�6I   $	��Top=�6]   $	��332��6u   $��27��6�   ���	#FFF0EEEE6�   5      5      