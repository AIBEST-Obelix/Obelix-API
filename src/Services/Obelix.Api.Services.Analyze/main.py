import os
import sys
from fastapi import FastAPI
import dotenv

dotenv.load_dotenv()

from Routers.Analyze import router as analyze_router

if sys.stdout.encoding.lower() != "utf-8":
    sys.stdout.reconfigure(encoding="utf-8")

app = FastAPI()
app.include_router(analyze_router)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=8111)