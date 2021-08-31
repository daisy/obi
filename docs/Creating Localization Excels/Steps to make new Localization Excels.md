**Steps to make new Localization Excels: -**

You will require Easy Localizer to follow below steps. Easy Localizer can be downloaded from [http://www.foss.kharkov.ua/download/downloads/free/Easy%20Localizer/Easy%20Localizer%201.3.0.zip](http://www.foss.kharkov.ua/download/downloads/free/Easy%20Localizer/Easy%20Localizer%201.3.0.zip)

Step 1: - Copy English resource files (of Dialogs for example) in a separate folder and name it Dialog Translation for future reference.

Step 2: - Open Easy Localizer.

Step 3: - Select Dialog Translation folder in Easy Localizer and press next.

Step 4: - Select &quot;Export Resources to excel&quot; radio button and press next.

Step 5: - Select &quot;Dialog Translation folder&quot; and put the name of the excel say &quot;Dialog Translation Excel&quot;.

Step 6: - Press Next and continue. Excel will be created with first Column &quot;Default&quot; having English Terms. Put &quot;Translation&quot; as heading on column B.

Step 7: - Go through the each excel sheet and hide terms like Obi Help.chm (as apart from French other localizations do not have localized help file) which you do not want to be translated.

Step 8: - Put &quot;[DAISY book structure term]&quot; in the translation column for the terms like annotation, endnote etc.

Step 9: - Do the same process for making excel files for Project View, messages, Obi form, User controls, Pipeline &amp; peak meter.

Step 10:- As excel created by Easy Localizer is in xls format so use &quot;Save as&quot; to save excel in xlsx format.

Note: -

1. In Project View Excel, we use all project view resource files.
2. In Dialog Excel, we use all dialog resource file.
3. For Obi Form Excel, we use ObiForm.resx.
4. For User Control, we use RecordingToolBarForm.resx and TextVUMeterPanel.resx.
5. For messages, we use messages.resx of Obi.
6. For Pipeline &amp; Peak meter, we use PipelineInterfaceForm.resx and messages.resx from pipeline interface project and PeakMeterForm.resx from Obi project

**Steps to make new Localization Excel for About and Profile Description Pages: -**

Step 1: - Open English html file of Profile Description and About.

Step 2: - Make a new excel sheet with the Heading on first column as &quot;Default&quot; and second Column as &quot;Translation&quot;

Step 3: - Copy the English terms from html file to excels first column &quot;Default&quot; .

**Note:** It is quite possible that when you press next in Step 6 you will get error. This error normally comes because Easy reader do not support .NET 4 resource files.

To fix this error follow following steps:-

Step 1:- Open the resource file in notepad++ which is causing error in Easy Reader.

Step 2: Use replace all feature of notepad++ to replace all instances of &quot;Version=4.0.0.0&quot; with &quot;Version=2.0.0.0&quot;. Do this procedure with all the resource files giving error.