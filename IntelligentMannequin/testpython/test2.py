import http.client, urllib.request, urllib.parse, urllib.error, base64

headers = {
    # Request headers
    'Content-Type': 'application/json',
    'Ocp-Apim-Subscription-Key': 'fa478a4258b3437f87dd0c5a62346c96',
}

params = urllib.parse.urlencode({
    # Request parameters
    'visualFeatures': 'Faces',
    'language': 'en',
})

try:
    conn = http.client.HTTPSConnection('api.projectoxford.ai')
    conn.request("POST", "/vision/v1.0/analyze?%s" % params, "{\"url\": \"https://intellimannstorage.blob.core.windows.net/myimages/myblob\"}", headers)
    response = conn.getresponse()
    data = response.read()
    print(data)
    conn.close()
except Exception as e:
    print("[Errno {0}] {1}".format(e.errno, e.strerror))
