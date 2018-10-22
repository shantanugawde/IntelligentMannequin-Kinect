from azure.storage.blob import BlockBlobService

count=0
block_blob_service = BlockBlobService(account_name='intellimannstorage', account_key='agA19rC5bc9JjbcCb8vyya/OqXCxrP4slPOG8L+dgVmdmKk3HRbgFu+f2W8eZDhc/uiMhadKIZseL9z1nHFwow==')
generator = block_blob_service.list_blobs('myimages')

for blob in generator:
    print(blob.name)
    block_blob_service.get_blob_to_path('myimages', blob.name, 'out-'+str(count)+'.jpg')