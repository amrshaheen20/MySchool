import random
from faker import Faker
from datetime import timezone
import json
fake = Faker()

def LoadData(Path):
    with open(Path, 'r', encoding='utf-8') as f:
        data = json.load(f)
    return data


def generate_random_guardians(nums=5, users=None):
    if users is None:
        users = []

    student_guardians = []
    
    students = [user for user in users if user["Role"] == "Student"]
    guardians = [user for user in users if user["Role"] == "Guardian"]

    max_pairs = min(nums, len(students))
    selected_students = random.sample(students, max_pairs)

    for student in selected_students:
        guardian = random.choice(guardians)

        student_guardians.append({
            "StudentId": student["Id"],
            "GuardianId": guardian["Id"],
            "RelationToStudent": "Guardian"
        })
    
    return student_guardians


if __name__ == "__main__":
    users = LoadData( "../Users/users.json")
    random_guardians = generate_random_guardians(100,users)
    
    file_path = "Guardians.json"
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(random_guardians, f, indent=4, ensure_ascii=False)
    print(f"Generated {len(random_guardians)} guardians and saved to {file_path}.")