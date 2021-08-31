**Steps to import Localized Excels in the project: -**

You will require Easy Localizer to follow below steps. Easy Localizer can be downloaded from [http://www.foss.kharkov.ua/download/downloads/free/Easy%20Localizer/Easy%20Localizer%201.3.0.zip](http://www.foss.kharkov.ua/download/downloads/free/Easy%20Localizer/Easy%20Localizer%201.3.0.zip)

Step 1: - Copy two sets of English resource files in a separate folder. I will be using Dialogs for explanation.

Step 2: - Now rename one set of English resource files to include culture code of the target localization. For example, if we are importing French then about.resx will be renamed to about.fr.resx. Make sure that for each dialog there are two resource files i.e for about dialog there should be both about.resx and about.fr.resx in the folder.

Step 3: - Copy the translated excel (send by translator to us) inside this folder.

Step 4: - Open Translated excel and change the heading of the second column from &quot;Translation&quot; to culture code. For example, if it is French then change &quot;Translation&quot; to &quot;fr&quot;.

Step 5: - Go through each sheet in the excel and check for any wrong or unexpected entry like something written on the third column of the excel.

Note: If you forgot to change &quot;Translation&quot; to &quot;fr&quot; then Easy Localizer will throw an error while importing.

Step 6: - Now open easy localizer.

Step 7: - Select the folder in which you have copied Localization files and press next.

Step 8: - Select &quot;Import resources from excel&quot; and press next.

Step 9: - If there was no error then localization will be successfully imported and you are done.

Note: - Sometimes you can get some errors while importing. The reason for which is reported by Easy Localizer. Some common errors which I have noticed are: -

1. If a translator has put something on the third column, then Easy Localizer will throw error.
2. Sometimes a translator puts some garbage value in the first column of the excel. This column is normally hidden when you make an excel using easy localizer. So, to check you need to unhide Rows and Columns.

Step 10: - Now copy Localized resource files to the main project folder. Remember to copy only localized files with culture code extension and not English resource files. For example, for dialogs translation in French you will copy all the localized French files with \*.fr.resx in the dialog folder of the main project.

**Note:** Importing &quot;About Obi Translation&quot; and &quot;Profile Description Translation&quot; excels is done manually. Data in these excels need to be dumb in html files.