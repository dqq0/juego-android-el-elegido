import re
import glob

scenes = glob.glob("Assets/**/*.unity", recursive=True)

for scene_path in scenes:
    print(f"\n==========================================")
    print(f"Inspecting scene: {scene_path}")
    print(f"==========================================")
    with open(scene_path, 'r', encoding='utf-8') as f:
        content = f.read()

    objects = content.split("--- !u!")
    mono_behaviours = []
    for obj in objects:
        if obj.startswith("114 &"): # MonoBehaviour
            lines = obj.splitlines()
            header = lines[0]
            script_line = None
            gameobject_line = None
            name_line = None
            
            for line in lines:
                if "m_Script:" in line:
                    script_line = line.strip()
                elif "m_GameObject:" in line:
                    gameobject_line = line.strip()
                elif "m_Name:" in line:
                    name_line = line.strip()
            
            mono_behaviours.append({
                "header": header,
                "script": script_line,
                "gameobject": gameobject_line,
                "name": name_line,
                "full": obj
            })

    # Print Monobehaviours that have script guid or look interesting
    custom_mbs = 0
    for idx, mb in enumerate(mono_behaviours):
        script = mb["script"]
        is_standard = False
        for std_guid in [
            "59f8146938fff824cb5fd77236b75775", # layout
            "5f7201a12d95ffc409449d95f23cf332", # text
            "4e29b1a8efbd4b44bb3f3716e73f07ff", # button
            "fe87c0e1cc204ed48ad3b37840f39efc", # image
            "a79441f348de89743a2939f4d699eac1", # canvas
            "30649d3a9faa99c48a7b1166b86bf2a0", # canvas scaler
            "dc42784cf147c0c48a680349fa168899", # graphic raycaster
            "0cd44c1031e13a943bb63640046fad76", # canvas scaler/ui
            "e19747de3f5aca642ab2be37e372fb86", # shadow
            "76c392e42b5098c458856cdf6ecaaaa1", # outline/shadow
        ]:
            if script and std_guid in script:
                is_standard = True
                break
                
        if not is_standard:
            custom_mbs += 1
            print(f"\n[{idx}] {mb['header']}")
            print(f"  GameObject: {mb['gameobject']}")
            print(f"  Script: {mb['script']}")
            # print all lines of mb["full"]
            lines = mb["full"].splitlines()
            for l in lines[:20]:
                if any(x in l.lower() for x in ["story", "music", "sound", "dialogue", "txt", "btn", "img", "screen"]):
                    print(f"    {l.strip()}")
                    
    print(f"Total custom MonoBehaviours found: {custom_mbs}")
