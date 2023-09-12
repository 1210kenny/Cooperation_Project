# -*- coding: UTF-8 -*-

import os
import sys
from langchain.llms import OpenAI
from langchain.agents import load_tools
from langchain.agents import initialize_agent
from langchain.llms import OpenAI
from langchain.agents import AgentType

def hw(question):
    bot_reply = agent.run(question)

    return bot_reply

if __name__ == '__main__':
    os.environ["OPENAI_API_KEY"] = sys.argv[1]
    os.environ["SERPAPI_API_KEY"] = sys.argv[2]
    que = sys.argv[3]

    # 加载 OpenAI 模型
    llm = OpenAI(temperature=0, max_tokens=2048, model_name="text-davinci-003")
    # 加载 serpapi 工具
    tools = load_tools(["serpapi"])
    # 工具加载后都需要初始化，verbose 参数为 True，会打印全部的执行详情
    agent = initialize_agent(tools, llm, agent=AgentType.ZERO_SHOT_REACT_DESCRIPTION, verbose=False)

    ans = hw(que)
    sys.stdout.reconfigure(encoding='utf-8')
    print(ans)

