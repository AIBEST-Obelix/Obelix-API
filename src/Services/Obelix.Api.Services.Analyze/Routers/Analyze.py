import json
import os
from typing import List

from fastapi import APIRouter, UploadFile, File, HTTPException
from pydantic import ValidationError

from Models.Item import Item, ItemResponse
from Services.ImageService.ImageService import image_service
from Services.GoogleNlpService import google_nlp_service

import base64

router = APIRouter()

@router.post("/item/analyze")
async def analyze_item(files: List[UploadFile] = File(...)) -> ItemResponse:
    if not files:
        raise HTTPException(
            status_code=400,
            detail="No files provided"
        )

    try:
        combined_image = await image_service.combine_images(files)

        item = await google_nlp_service.parse_item(base64.b64encode(combined_image.read()))

        item_response = ItemResponse(Name=item.name, Type=item.type, SerialNumber=item.serial_number)

        return item_response

    except ValidationError as e:
        raise HTTPException(
            status_code=400,
            detail=f"Invalid item data: {str(e)}",
        )

    except json.JSONDecodeError as e:
        raise HTTPException(
            status_code=400,
            detail=f"Failed to decode JSON response: {str(e)}",
        )

    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=f"An unexpected error occurred: {str(e)}",
        )