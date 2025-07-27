import json
import os
import openai
from google.oauth2 import service_account
from google.auth.transport.requests import Request

from Models.Item import Item

class GoogleNlpService:
    def __init__(self):
        PROJECT_ID = "llama-4-scout"
        LOCATION = "us-east5"
        self._MODEL_ID = "meta/llama-4-maverick-17b-128e-instruct-maas"

        SCOPES = ['https://www.googleapis.com/auth/cloud-platform']
        credentials_path = os.environ.get('GOOGLE_APPLICATION_CREDENTIALS')
        credentials = service_account.Credentials.from_service_account_file(
            credentials_path,
            scopes=SCOPES
        )

        request = Request()
        credentials.refresh(request)
        gcp_token = credentials.token

        vertex_ai_endpoint_url = (
            f"https://{LOCATION}-aiplatform.googleapis.com/v1beta1/"
            f"projects/{PROJECT_ID}/locations/{LOCATION}/endpoints/openapi"
        )

        self._client = openai.OpenAI(
            base_url=vertex_ai_endpoint_url,
            api_key="dummy_key",
            default_headers={
                "Authorization": f"Bearer {gcp_token}"
            }
        )

        self._PROMPT = '''You are given the image of an item.
Your task is to extract information about the item and return a JSON object with the following fields:
- name: The name of the item.
- type: The general type of the item.
- serial_number: The serial number of the item. (optional, can be empty)'''


    async def parse_item(self, image_base64) -> Item:
        if isinstance(image_base64, bytes):
            print("Image is in bytes format, decoding...")
            image_base64 = image_base64.decode('utf-8')

        data_uri = f"data:image/jpeg;base64,{image_base64}"

        messages = [
            {
                "role": "user",
                "content": [
                    {"type": "text", "text": self._PROMPT},
                    {"type": "image_url", "image_url": data_uri}
                ]
            }
        ]

        response = self._client.chat.completions.create(
            model=self._MODEL_ID,
            messages=messages,
            temperature=0.0,
            top_p=1.0,
            response_format={
                "type": "json_object"
            }
        )

        response_json = response.choices[0].message.content.strip()

        print("Response JSON:", response_json)

        item_dict = json.loads(response_json)
        item = Item(**item_dict)

        return item


google_nlp_service = GoogleNlpService()

__all__ = ["google_nlp_service"]
