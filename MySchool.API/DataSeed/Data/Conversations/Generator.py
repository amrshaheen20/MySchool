import random
from faker import Faker
from datetime import timezone
import json
fake = Faker()

def LoadData(Path):
    with open(Path, 'r', encoding='utf-8') as f:
        data = json.load(f)
    return data

def generate_random_messages(nums=5, users=[]):
    messages = []
    
    for _ in range(nums):
        user = random.choice(users)
        message = {
            "Content": fake.text(max_nb_chars=100),
            "UserId": user["Id"],
        }
        messages.append(message)
    
    return messages


def generate_random_conversations(nums=5, users=[]):
    conversations = []
    
    for _ in range(nums):
        user_one = random.choice(users)
        user_two = random.choice([user for user in users if user != user_one])

        conversation = {
            "Id":_+1,
            "UserOneId": user_one["Id"],
            "UserTwoId": user_two["Id"],
            "Messages": generate_random_messages(300, users=[user_two, user_one])
        }
        conversations.append(conversation)
    
    return conversations


if __name__ == "__main__":
    users = LoadData( "../Users/users.json")
    random_conversations = generate_random_conversations(100,users)
    
    file_path = "conversations.json"
    with open(file_path, 'w', encoding='utf-8') as f:
        json.dump(random_conversations, f, indent=4, ensure_ascii=False)
    print(f"Generated {len(random_conversations)} conversations and saved to {file_path}.")