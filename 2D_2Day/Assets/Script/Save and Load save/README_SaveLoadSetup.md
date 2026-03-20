# Save & Load System - การตั้งค่า

## โครงสร้าง
- **IDataPersistence** – interface สำหรับ component ที่ต้องการ Save/Load
- **GameData** – โครงสร้างข้อมูลที่เก็บเป็น JSON (storyFlags, completedEventNames, สถานที่, บท, เวลาเล่น, วันที่)
- **FileDataHandler** – อ่าน/เขียนไฟล์ JSON และรูป thumbnail
- **DataPersistenceManager** – Singleton: New Game, Save, Load, ClearSaveData และ List<IDataPersistence>
- **GameDataProvider** – เก็บ currentLocation, chapterTitle ให้ตั้งค่าจาก level/trigger
- **PlayTimeTracker** – นับเวลาเล่นและบันทึกใน GameData
- **SaveLoadUI** – หน้า UI ช่อง Save (thumbnail, วันที่, สถานที่, บท, เวลา) + ปุ่ม Back, Save
- **SaveSlotUI** – หนึ่งแถวต่อหนึ่งช่อง save
- **SaveTrigger** – ใส่ที่ Obj ในเกม เมื่อกด/เข้า trigger จะเปิดหน้า Save
- **MainMenuUI** – ปุ่ม New Game, Continue, Option, Quit
- **ClearSaveDataButton** – ปุ่มสำหรับล้างข้อมูล Save (ใช้ทดสอบ)
- **Tools > Save & Load > Clear Save Data** – ล้าง Save จากเมนูใน Unity Editor (ใช้ตอนทดสอบ)
- **Tools > Save & Load > Open Save Data Folder** – เปิดโฟลเดอร์ที่เก็บไฟล์ JSON

## การตั้งค่าใน Scene

### 1. Bootstrap / ฉากแรก (หรือ DontDestroyOnLoad)
- สร้าง GameObject ว่าง เช่น `SaveLoadBootstrap`
- เพิ่ม Component: **DataPersistenceManager**, **PlayTimeTracker**, **GameDataProvider**
- DataPersistenceManager ตั้ง `maxSaveSlots = 3` (หรือตามจำนวนช่องที่ใช้)

### 2. EventManager
- EventManager มี IDataPersistence อยู่แล้ว แค่ให้มี **DataPersistenceManager** อยู่ในเกม (ฉากแรกหรือ DontDestroyOnLoad) EventManager จะ register ตัวเองเมื่อ Awake

### 3. หน้า Save (ตามที่ออกแบบ)
- สร้าง Canvas/Panel หน้า Save
- แต่ละช่อง Save: มี RawImage (thumbnail), Text วันที่+สถานที่, Text บท+เวลาเล่น, ปุ่มเลือกช่อง
- เพิ่ม **SaveSlotUI** ที่แต่ละแถวช่อง (ลาก Ref ไปที่ RawImage, Text, ปุ่ม)
- ที่ Panel หลัก: เพิ่ม **SaveLoadUI** ใส่ List ของ SaveSlotUI, ปุ่ม Back, ปุ่ม Save

### 4. Obj เปิดหน้า Save
- ที่ Obj ที่ต้องการให้กดแล้วเข้าหน้า Save: เพิ่ม **SaveTrigger**
- กำหนด `saveLoadUIPanel` = Panel หน้า Save
- ถ้าต้องการกดปุ่มเปิด: ตั้ง `openKey` (เช่น E) และใช้ Collider กันพื้นที่กด หรือใช้ปุ่ม UI เรียก `SaveTrigger.OpenSaveMenu()`

### 5. Main Menu
- หน้า Main Menu: ปุ่ม New Game, Continue, Option, Quit
- เพิ่ม **MainMenuUI** กำหนด ref ปุ่มและ (ถ้ามี) SaveLoadPanel
- ตั้ง `firstGameSceneName` = ชื่อ Scene เกม (เช่น SampleScene)
- ถ้าต้องการให้ Continue เปิดหน้าเลือกช่อง Save แทนโหลดล่าสุด: เปิด `continueOpensSavePanel` และใส่ SaveLoadPanel

### 6. ตั้งค่าสถานที่ / บท
- ในเกมเมื่อเข้าแต่ละพื้นที่ เรียก:
  - `GameDataProvider.Instance.SetLocation("ร้านบาร์ของหลิน");`
  - `GameDataProvider.Instance.SetChapter("พบเจอ", 1);`
- หรือใส่ค่าใน Inspector ที่ GameDataProvider ถ้าไม่เปลี่ยนตามฉาก

### 7. ล้าง Save ตอนทดสอบ
- **ใน Unity Editor:** เมนู **Tools > Save & Load > Clear Save Data** (ไม่ต้องเข้า Play mode ก็ล้างได้)
- ในเกม (Option หรือที่ใดก็ได้): สร้างปุ่ม แล้วเพิ่ม Component **ClearSaveDataButton**

## หมายเหตุ
- ไฟล์ Save (JSON + thumbnail) อยู่ที่ `Application.persistentDataPath/Save Data Json/` หาได้ง่าย
- หน้า Save/Load มีปุ่ม **Back**, **Save**, **Load** — เลือกช่องแล้วกด Save หรือ Load
