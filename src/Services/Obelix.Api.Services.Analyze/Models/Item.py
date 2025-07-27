from pydantic import BaseModel

class Item(BaseModel):
    name: str
    type: str
    serial_number: str

class ItemResponse(BaseModel):
    Name: str
    Type: str
    SerialNumber: str