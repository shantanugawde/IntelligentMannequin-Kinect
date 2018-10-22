from azure.storage.blob import BlockBlobService
import http.client, urllib.request, urllib.parse, urllib.error, base64, json
import matplotlib.pyplot as plt, numpy as np

block_blob_service = BlockBlobService(account_name='intellimannstorage', account_key='agA19rC5bc9JjbcCb8vyya/OqXCxrP4slPOG8L+dgVmdmKk3HRbgFu+f2W8eZDhc/uiMhadKIZseL9z1nHFwow==')
generator = block_blob_service.list_blobs('myimages')

generator2 = block_blob_service.list_blobs('myinfo')
block_blob_service.get_blob_to_path('myinfo', 'info.txt', 'stats.txt')


file = open("stats.txt", "r")
mydata = file.read()
mydat = mydata.split()
totalCount = mydat[0]
interestCount = mydat[1]

print("Total people: ", totalCount)
# print(interestCount)

count_male = 0
count_female = 0
neutralCount = 0
happyCount = 0
age30 = 0
age15 = 0

headersCV = {
    # Request headers
    'Content-Type': 'application/json',
    'Ocp-Apim-Subscription-Key': '2750290ac9974f1d9bc4086920395539',
}

headersEmo = {
    # Request headers
    'Content-Type': 'application/json',
    'Ocp-Apim-Subscription-Key': '51201952e1044ad19a46b5638168a56f',
}

paramsCV = urllib.parse.urlencode({
    # Request parameters
    'visualFeatures': 'Faces',
    'language': 'en',
})

paramsEmo = urllib.parse.urlencode({
})

for blob in generator:
    try:
        conn1 = http.client.HTTPSConnection('api.projectoxford.ai')
        conn1.request("POST", "/vision/v1.0/analyze?%s" % paramsCV, "{\"url\": \"https://intellimannstorage.blob.core.windows.net/myimages/"+blob.name+"\"}", headersCV)
        conn2 = http.client.HTTPSConnection('api.projectoxford.ai')
        conn2.request("POST", "/emotion/v1.0/recognize?%s" % paramsEmo, "{\"url\": \"https://intellimannstorage.blob.core.windows.net/myimages/"+blob.name+"\"}", headersEmo)
        response1 = conn1.getresponse()
        response2 = conn2.getresponse()
        dataCV = response1.read().decode('utf-8')
        dataEmo = response2.read().decode('utf-8')
        r = json.loads(dataEmo)
        # print(r)
        maxRating = -1
        for p in r:
            for key, value in p["scores"].items():
                if value > maxRating:
                    maxRating = value
                    emotion = key
                # print(key, value)
            print(emotion, maxRating)
            if emotion == "neutral":
                neutralCount+=1
            elif emotion == "happiness":
                happyCount+=1
            maxRating = -1
            emotion = "None"
        obj = json.loads(dataCV)
        for p in obj["faces"]:
            if(p["gender"] == "Male"):
                count_male = count_male + 1
                print("Males :", count_male)
            if(p["gender"] == "Female"):
                count_female = count_female + 1
                print("Females :", count_female)
            if(p["age"] > 30):
                age30+=1
            elif(p["age"] <= 30 and p["age"] > 15):
                age15+=1
        conn1.close()
        conn2.close()
    except Exception as e:
        print("[Errno {0}] {1}".format(e.errno, e.strerror))

f = plt.figure(1)
x = [0,1]
y = [count_male, count_female]
st = "Male:",count_male," Female:",count_female
plt.title(st)
plt.bar(x,y,0.2,align="center", color="red")
axes =  plt.gca()
plt.xticks([0,1],['Male','Female'])
#axes.set_xlim(0,3)
f.show()

g = plt.figure(2)
x = [0,1]
y = [happyCount, neutralCount]
st = "Happy:",happyCount," Neutral",neutralCount
plt.title(st)
plt.bar(x,y,0.2,align="center", color="red")
axes = plt.gca()
plt.xticks([0,1],['Happy','Neutral'])
#axes.set_xlim(0,3)
g.show()

h = plt.figure(3)
x = [0,1]
y = [age15, age30]
st = "Age<30:",age15," Age>30",age30
plt.title(st)
plt.bar(x,y,0.2,align="center", color="red")
axes = plt.gca()
plt.xticks([0,1],['Age<30','Age>30'])
#axes.set_xlim(0,3)
h.show()

input()