import random
from faker import Faker
from datetime import timezone
import json
fake = Faker()

roles = ["Admin", "Teacher", "Student", "Guardian"]
genders = ["Male", "Female"]
male_names = ["Mohamed", "Ahmed", "Ali", "Omar", "Youssef", "Hassan", "Ibrahim", "Khaled", "Tamer", "Amr","Kamal"]
female_names = ["Fatma", "Sara", "Aisha", "Mona", "Nadia", "Hana", "Dina", "Rania", "Yasmin", "Laila","Nesreen","Riham"]

def generate_random_name(gender):
    if gender=='Male':
        return random.choice(male_names)+" "+random.choice(male_names)+" "+random.choice(male_names)
    else:
        return random.choice(female_names)+" "+random.choice(male_names)+" "+random.choice(male_names)
    
def egyptian_phone_number():
    prefix = random.choice(["010", "011", "012", "015"])
    number = ''.join([str(random.randint(0, 9)) for _ in range(8)])
    return prefix + number


def generate_random_users(num_users=5):
    users = []
    

    
    for _ in range(num_users):
        gender = random.choice(genders)
        name = generate_random_name(gender)
        username = str(fake.unique.random_number(digits=8, fix_len=True))
        role = random.choice(roles)
        must_change_password = random.choice([True, False])
        is_active = random.choice([True, False])
        national_id =str(fake.unique.random_number(digits=14))
        gender = random.choice(genders)
        date_of_birth =  role =='Student' and fake.date_of_birth(minimum_age=3, maximum_age=18).isoformat() or \
                        (role == 'Guardian' or role == 'Teacher'or role == 'Admin') and fake.date_of_birth(minimum_age=18, maximum_age=70).isoformat() 
        
        address = fake.address().replace("\n", ", ")
        phone_number = egyptian_phone_number()
        last_active_time = fake.date_time_this_year(tzinfo=timezone.utc).isoformat()
        
        custom_fields = [
            {"Key": "Department", "Value": fake.job()},
        ]
        
        user = {
            "Id": _ + 1,
            "Name": name,
            "UserName": username,
            "Role": role,
            "Password": "123456789",
            "MustChangePassword": must_change_password,
            "IsActive": is_active,
            "NationalId": national_id,
            "Gender": gender,
            "DateOfBirth": date_of_birth,
            "Address": address,
            "PhoneNumber": phone_number,
            "LastActiveTime": last_active_time,
           # "CustomFields": custom_fields
        }
        
        users.append(user)
    
    return users


if __name__ == "__main__":
    random_users = generate_random_users(100)
    
    file_path = "users.json"
    with open(file_path, 'w') as f:
        json.dump(random_users, f, indent=4)
    print(f"Generated {len(random_users)} random users and saved to {file_path}.")

