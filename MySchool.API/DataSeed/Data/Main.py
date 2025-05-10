from Announcements.Generator import generate_random_announcements
from ClassRooms.Generator import generate_random_classes
from Conversations.Generator import generate_random_conversations
from Guardians.Generator import generate_random_guardians
from Users.Generator import generate_random_users 
from Subjects.Generator import generate_random_subjects
import json


def SaveData(path, data):
    with open(path, 'w', encoding='utf-8') as f:
        json.dump(data, f, indent=4, ensure_ascii=False)


if __name__ == "__main__":

    print("Generating data...")
    ####################################################################
    # Generate random data
    ####################################################################
    users=generate_random_users(500)
    subjects=generate_random_subjects(10)
    classes=generate_random_classes(20, users, subjects)
    announcements=generate_random_announcements(200,users)
    conversations=generate_random_conversations(200,users)
    generates=generate_random_guardians(40,users)


    ####################################################################
    # Save data to JSON files
    ####################################################################
    SaveData("Users/users.json", users)
    SaveData("Subjects/subjects.json", subjects)
    SaveData("ClassRooms/classes.json", classes)
    SaveData("Announcements/announcements.json", announcements)
    SaveData("Conversations/conversations.json", conversations)
    SaveData("Guardians/Guardians.json", generates)
    print("Done")
