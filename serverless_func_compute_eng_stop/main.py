import os
from twilio.rest import Client
import functions_framework

def sms_sender(client, sender, recipient, message):
    client.messages.create(body = message, from_ = sender, to = recipient)


@functions_framework.http
def hello_http(request):
#    twil_account_sid = 'SID'
#    twil_auth_token = 'tokenm'
#    twil_phone_nr = '0664'
#    twil_target_nr = '0446'
    twil_service_enable = os.environ.get('TWIL_ENABLE')
    
    """HTTP Cloud Function.
    Args:
        request (flask.Request): The request object.
        <https://flask.palletsprojects.com/en/1.1.x/api/#incoming-request-data>
    Returns:
        The response text, or any set of values that can be turned into a
        Response object using `make_response`
        <https://flask.palletsprojects.com/en/1.1.x/api/#flask.make_response>.
    """
    url = 'POST https://compute.googleapis.com/compute/v1/projects/fhstp-sims/zones/ZONE/instances/INSTANCE_NAME/stop'

    request_json = request.get_json(silent=True)
    request_args = request.args

    if request_json and 'name' in request_json:
        name = request_json['name']
    elif request_args and 'name' in request_args:
        name = request_args['name']
    else:
        name = 'World'
    
    if twil_service_enable == 'true':
        twil_account_sid = os.environ.get('TWIL_ACC_SID')
        twil_auth_token = os.environ.get('TWIL_AUTH_TOKEN')
        twil_phone_nr = os.environ.get('TWIL_PH_NR')
        twil_target_nr = os.environ.get('TWIL_TARGET_NR')
        client_twil = Client(twil_account_sid, twil_auth_token)
        sms_sender(client_twil, twil_phone_nr, twil_target_nr, name)
    return 'Hello {}!'.format(name)





