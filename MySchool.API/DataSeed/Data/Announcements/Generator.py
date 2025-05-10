import random
from faker import Faker
from datetime import timezone
import json
fake = Faker()

def LoadData(Path):
    with open(Path, 'r', encoding='utf-8') as f:
        data = json.load(f)
    return data


def generate_random_announcements(nums=10, users=[]):
    announcements = []
    
    Users = [user for user in users if user["Role"] == "Student"]
    creators = [user for user in users if user["Role"] == "Admin"]

    for _ in range(nums):
        user = random.choice(Users)
        creator = random.choice(creators)

        announcement = {
            "Id":_+1,
            "UserId": user["Id"],
            "CreatedById": creator["Id"],
            "Title": fake.sentence(nb_words=6)[:100],
            "Content": fake.paragraph(nb_sentences=3)[:200] 
        }

        announcements.append(announcement)

    return announcements


if __name__ == "__main__":
    users = LoadData( "../Users/users.json")
    random_announcements = generate_random_announcements(100,users)
    
    file_path = "announcements.json"
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(random_announcements, f, indent=4, ensure_ascii=False)
    print(f"Generated {len(random_announcements)} announcements and saved to {file_path}.")