# SGGG BIN Translation Tool

*A utility for translating the main game binary of **SGGG** after applying the required binary editing.*


---

## 📌 Overview

This tool provides a simple workflow for translating the main SGGG game binary (`1_SGGG.BIN`).
Since the original binary uses **Shift-JIS encoding** and a fixed-width Japanese font layout, you must apply an **xdelta patch** beforehand.
This patch converts the game to use normal fonts suitable for English and expands the binary to fit longer strings (Only version v1.022 of the game is compatible).
Some notable patches:
* **megavolt85**: Half wide font 12x24 and support ASCII charset
* **Derek Pascarella aka ateam**: Hacked bios font func and extended the main bin file, fix for char name entry
* **MadSheep**: Skill space fix and multiple choice dialog space fix

After xdelta patch, the tool allows you to:

* View and translate text entries from the binary
* Save translation progress
* Generate a fully patched, playable translated binary

Note: the provided **sggg_data.txt** contains the translations the english translation project made

---

## 📂 Required Files

Make sure the following files are present:

| File                        | Description                                                   |
| --------------------------- | ------------------------------------------------------------- |
| `1_SGGG.BIN`                | main game binary (created using the xdelta patch) |
| `sggg_bin_translation2.exe` | The translation utility                                       |
| `sggg_data.txt`             | CSV-like table with all string addresses inside the binary    |

---

## 🚀 Usage Instructions

### **1. Patch the Original Binary**

Apply the provided xdelta patch to the original `1_SGGG.BIN`.
This produces the modified binary required for translation.


```
Original 1_SGGG.BIN → 1_SGGG.BIN.xdelta → Patched 1_SGGG.BIN
```

---

### **2. Prepare the Working Directory**

Place these files together in the **root folder** of the project:

* `sggg_bin_translation2.exe`
* `1_SGGG.BIN` (created using the xdelta patch)
* `sggg_data.txt`

---

### **3. Translate In-Tool**

Run:

```
sggg_bin_translation2.exe
```

You can now browse, edit, and translate all text strings defined in `sggg_data.txt`.<br/>
**Note:** If the bytes field is highlighted in red, it indicates that a pointer is required for text 
relocation. If the pointers column is unpopulated, you can press the “Find Pointers” button to 
allow the program to automatically locate and populate the required entries.

---

### **4. Save Translation Progress**

Click **Save** to write changes to `sggg_data.txt`.
This updates the translation and address table.

---

### **5. Generate the Translated Binary**

Click **Patch** to output:

```
1_SGGG.PATCHED.BIN
```

This file is the fully translated, playable version of the game’s main binary.

---

## 📁 Output

| Output File          | Purpose                                            |
| -------------------- | -------------------------------------------------- |
| `sggg_data.txt`      | Updated translation entries and address data       |
| `1_SGGG.PATCHED.BIN` | Final translated binary with applied modifications |

---

## 📝 Notes

* Always keep backups of the original and patched binaries.
* If the structure of the binary changes, you must update `sggg_data.txt`.
* The tool **only works on the patched** version of the binary, not the original.

---

## 🤝 Credits

Developed to support the English translation and modification of the SGGG main binary.

Thanks to:
* megavolt85 (bin hacks)
* Derek Pascarella aka ateam (bin hacks)
* pomegd (bin text dump)
* Exxistance (translation, image edits)
* VincentNL (mrg editor)
* mr.nobody (video edits, image edits)
* Sixfortyfive (translation review)
* LostinLoc 
* LewisJFC (DCJY)

