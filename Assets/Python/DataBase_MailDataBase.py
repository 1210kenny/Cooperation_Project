import os
import sys

from langchain.embeddings.openai import OpenAIEmbeddings
from langchain.vectorstores import Chroma
from langchain.text_splitter import CharacterTextSplitter
from langchain import OpenAI, VectorDBQA
from langchain.document_loaders import DirectoryLoader
from langchain.chains import RetrievalQA
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain.chains import ConversationalRetrievalChain
from langchain.chat_models import ChatOpenAI
from langchain.document_loaders import JSONLoader

# openAI的Key
os.environ["OPENAI_API_KEY"] = sys.argv[1]
import json

# file_path='C:\Users\陳子嫚\Downloads\0806\my.json'
# data = json.loads(Path(file_path).read_text())
# 加载文件夹中的所有txt类型的文件

from langchain.document_loaders import DirectoryLoader, TextLoader

from pathlib import Path
from typing import Callable, Dict, List, Optional, Union

from langchain.docstore.document import Document
from langchain.document_loaders.base import BaseLoader


class JSONLoader(BaseLoader):
    def __init__(
            self,
            file_path: Union[str, Path],
            content_key: Optional[str] = None,
    ):
        self.file_path = Path(file_path).resolve()
        self._content_key = content_key

    def load(self) -> List[Document]:
        """Load and return documents from the JSON file."""

        docs = []
        # Load JSON file
        with open(self.file_path, encoding='utf-8') as file:
            data = json.load(file)

            # Iterate through 'pages'
            for target in data['target']:
                role = target['role']
                content = target['content']
                metadata = dict(
                    source=content,
                )
                docs.append(Document(page_content=content, metadata=metadata)

                            )
        return docs


file_path = r'EmailBox_Keys.json'
loader = JSONLoader(file_path=file_path)
# data = loader.load()
# loader = DirectoryLoader(r'C:\bfile', glob='**/*.txt')
# 将数据转成 document 对象，每个文件会作为一个 document
documents = loader.load()

# 初始化加载器
# text_splitter = CharacterTextSplitter(chunk_size=100, chunk_overlap=0)
# 切割加载的 document
# split_docs = text_splitter.split_documents(documents)
splitter = RecursiveCharacterTextSplitter(chunk_size=500, chunk_overlap=0)
texts = loader.load_and_split(splitter)
# 初始化 openai 的 embeddings 对象
embeddings = OpenAIEmbeddings()
# 将 document 通过 openai 的 embeddings 对象计算 embedding 向量信息并临时存入 Chroma 向量数据库，用于后续匹配查询
# docsearch = Chroma.from_documents(split_docs, embeddings)
vectorstore = Chroma.from_documents(texts, embeddings)
# 创建问答对象
# qa = VectorDBQA.from_chain_type(llm=OpenAI(), chain_type="stuff", vectorstore=docsearch, return_source_documents=False)
# 进行问答
qa = ConversationalRetrievalChain.from_llm(ChatOpenAI(temperature=0), vectorstore.as_retriever())
query = sys.argv[2]
result = qa({"question": query, 'chat_history': []})
sys.stdout.reconfigure(encoding='utf-8')
print(result['answer'])