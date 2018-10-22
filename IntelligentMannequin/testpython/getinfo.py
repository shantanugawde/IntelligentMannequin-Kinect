from azure.storage.queue import QueueService

queue_service = QueueService(account_name='intellimannstorage', account_key='agA19rC5bc9JjbcCb8vyya/OqXCxrP4slPOG8L+dgVmdmKk3HRbgFu+f2W8eZDhc/uiMhadKIZseL9z1nHFwow==')

messages = queue_service.get_messages('myqueue')
metadata = queue_service.get_queue_metadata('myqueue')
count = metadata.approximate_message_count
print(count)
for message in messages:
    print(__dir__(message))
    print(message.content)
    print(type(message.content))
    queue_service.delete_message('myqueue', message.id, message.pop_receipt)