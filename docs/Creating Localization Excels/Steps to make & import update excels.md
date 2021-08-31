**Steps to make update Excels: -**

You will require Easy Localizer to follow below steps. Easy Localizer can be downloaded from [http://www.foss.kharkov.ua/download/downloads/free/Easy%20Localizer/Easy%20Localizer%201.3.0.zip](http://www.foss.kharkov.ua/download/downloads/free/Easy%20Localizer/Easy%20Localizer%201.3.0.zip)

**Method 1: -**

Step 1: - Copy English and Localized resource files in a single folder. For example, if I need to make update excels for French then copy English and French resource files in a separate folder.

Step 2: - Now follow the steps mentioned to make Localized Excels.

Step 3: - Now open localized excels. In this you will notice that for the newly added terms there will be no translation. You can change the colors for such cells so that translator can easily translate those terms.

OR follow following steps after Step 2 for more cleaner solution

Step 3: Delete sheets which do not require any new Translation.

Step 4: Rename column &quot;fr&quot; to &quot;Translation&quot; in all the remaining sheets.

Step 5: In sheets which have new translation hide rows which do not require new translation such that only rows requiring translation are shown to the user.

**Method 2: -**

If there are only few terms that needs to be translated, then make a new excel manually and add those terms in it.

**Steps to import update excels: -**

**Method 1: -**

Step 1: - Copy English and Localized resource files from a main project to a single folder.

Step 2: - Now follow the steps mentioned to import Localized excels.

Step 3: - Run project to see the if there any major issues especially with in the dialogs where there are lot of changes.

Step 4: - Do repositioning of controls in dialogs if required.

**Method 2: -**

If you have made translation excel manually by method 2 as stated above, then you will need to include it manually.