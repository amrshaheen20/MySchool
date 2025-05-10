from faker import Faker
import random
import json

fake = Faker()

def generate_random_subjects(nums=5):
    subjects = []
    #lol -> I don't know what to put here I never went to school :D
    subject_names = [
        "Math", "Science", "History", "Geography", "English",
        "Physics", "Chemistry", "Biology", "Art", "Music",
        "Computer Science", "Physical Education", "Economics", "Psychology",
        "Sociology", "Philosophy", "Literature", "Statistics", "Astronomy",
        "Breaking Bad", "Game of Thrones", "Stranger Things",
        "The Office", "Friends", "The Simpsons", "Narcos", "Black Mirror",
        "The Crown", "The Mandalorian", "WandaVision", "The Witcher",
        "Amr"
    ]

    used_names = set()

    for _ in range(nums):
        subject_name = random.choice(subject_names)
        
        if subject_name in used_names:
            continue
        used_names.add(subject_name)

        subject = {
            "Id": _ + 1,
            "Name": subject_name,
            #"Description": fake.text(max_nb_chars=100)
        }
        subjects.append(subject)

    return subjects


if __name__ == "__main__":
    subjects = generate_random_subjects(nums=10)
    file_path = "subjects.json"
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(subjects, f, indent=4, ensure_ascii=False)
    print(f"Generated {len(subjects)} subjects and saved to {file_path}.")
    