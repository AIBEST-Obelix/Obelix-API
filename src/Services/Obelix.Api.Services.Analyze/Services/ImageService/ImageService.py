import io
from typing import List

from fastapi import UploadFile
from PIL import Image


class ImageService:
    """
    A class to handle images.
    """

    def __init__(self):
        pass

    async def combine_images(self, files: List[UploadFile]):
        images = [Image.open(io.BytesIO(file.file.read())) for file in files]

        max_width = max(img.width for img in images)
        total_height = sum(img.height for img in images)

        # create a new image with the maximum width and total height and in RGB mode
        combined_image = Image.new("RGB", (max_width, total_height))

        # paste each image into the combined image, centering them horizontally
        y_offset = 0
        for img in images:
            x_offset = (max_width - img.width) // 2
            combined_image.paste(img, (x_offset, y_offset))
            y_offset += img.height

        stream = io.BytesIO()
        combined_image.save(stream, format="JPEG")
        stream.seek(0)
        return stream


image_service = ImageService()

__all__ = ["image_service"]