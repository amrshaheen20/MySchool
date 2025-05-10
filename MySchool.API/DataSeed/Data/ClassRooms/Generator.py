import random
from faker import Faker
from datetime import timezone
import json
fake = Faker()

def LoadData(Path):
    with open(Path, 'r', encoding='utf-8') as f:
        data = json.load(f)
    return data




def generate_random_class_name():
    return  "Class " + str(random.randint(1, 10))+ "-" + str(random.randint(1, 100))

used_student_ids = set()

def generate_random_Enrollments(nums=5, users=[]):
    enrollments = []
    students = [user for user in users if user["Role"] == "Student"]
    unique_students = random.sample(students, min(nums, len(students)))
    for student in unique_students:
        if student["Id"] in used_student_ids:
            continue
        used_student_ids.add(student["Id"])
        enrollment = {
            "StudentId": student["Id"],
        }

        enrollments.append(enrollment)

    return enrollments



def generate_random_timetables(nums=5, users=[], subjects=[]):
    timetables = []
    teachers = [user for user in users if user["Role"] == "Teacher"]
    
    for _ in range(nums):
        start_hour = random.randint(8, 14) 
        start_minute = random.choice([0, 15, 30, 45])
        duration_minutes = random.choice([30, 45, 60])
        end_hour = start_hour + (duration_minutes // 60)
        end_minute = (start_minute + duration_minutes) % 60

        start_time = f"{start_hour:02}:{start_minute:02}"
        end_time = f"{end_hour:02}:{end_minute:02}"

        timetable = {
            "Id": _ + 1, 
            "SubjectId": random.choice(subjects)['Id'],
            "TeacherId": random.choice(teachers)['Id'],
            "Day": random.choice(["Monday", "Tuesday", "Wednesday", "Thursday", "Sunday"]),
            "StartTime": start_time,
            "EndTime": end_time
        }
        timetables.append(timetable)
    return timetables



def generate_random_attendance(nums=5, users=[], Students=[]):
    attendances = [] 
    admins = [user for user in users if user["Role"] == "Admin"]
    
    for _ in range(nums):
        studentid = random.choice(Students)
        created_by = random.choice(admins)
        status = random.choice(["Absent","Present","Late","Excused"])   
        
        date_of_attendance = fake.date_this_year().isoformat()

        
        attendance = {
            "CreatedById": created_by["Id"],
            "StudentId": studentid,
            "Date": date_of_attendance,
            "Status": status,
        }
        attendances.append(attendance)
    
    return attendances




def generate_random_classes(nums=5, users=[], subjects=[]):
    classes = []
    Enrollments =  generate_random_Enrollments(20, users)


    for _ in range(nums):
        class_info = {
            "Id": _ + 1,
            "Name": generate_random_class_name(),
            "Enrollments":Enrollments,
            "Timetables": generate_random_timetables(20, users, subjects),
            "Attendance": generate_random_attendance(20, users, [enrollment["StudentId"] for enrollment in Enrollments]),
        }
        
        classes.append(class_info)

    return classes


if __name__ == "__main__":
    users = LoadData( "../Users/users.json")
    subjects = LoadData( "../subjects/subjects.json")
    classes = generate_random_classes(100, users=users, subjects=subjects)
    file_path = "classes.json"
    with open(file_path, "w", encoding="utf-8") as f:
        json.dump(classes, f, indent=4, ensure_ascii=False)
    print(f"Generated {len(classes)} classes and saved to {file_path}.")