from azure.storage.blob import BlockBlobService
import http.client, urllib.request, urllib.parse, urllib.error, base64

block_blob_service = BlockBlobService(account_name='intellimannstorage', account_key='agA19rC5bc9JjbcCb8vyya/OqXCxrP4slPOG8L+dgVmdmKk3HRbgFu+f2W8eZDhc/uiMhadKIZseL9z1nHFwow==')
generator = block_blob_service.list_blobs('myimages')
headers = {
    # Request headers
    'Content-Type': 'application/json',
    'Ocp-Apim-Subscription-Key': '2750290ac9974f1d9bc4086920395539',
}

params = urllib.parse.urlencode({
    # Request parameters
    'visualFeatures': 'Faces',
    'language': 'en',
})

for blob in generator:
    try:
        conn = http.client.HTTPSConnection('api.projectoxford.ai')
        conn.request("POST", "/vision/v1.0/analyze?%s" % params, "{\"url\": \"https://intellimannstorage.blob.core.windows.net/myimages/"+blob.name+"\"}", headers)
        response = conn.getresponse()
        data = response.read()
        print(data)
        conn.close()
    except Exception as e:
        print("[Errno {0}] {1}".format(e.errno, e.strerror))
