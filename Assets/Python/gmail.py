import os
import sys


from langchain.llms import OpenAI
from langchain.agents import initialize_agent
from langchain.agents.agent_toolkits import ZapierToolkit
from langchain.utilities.zapier import ZapierNLAWrapper

def hw(question):
    bot_reply = agent.run(question)

    return bot_reply

if __name__ == '__main__':
    os.environ["OPENAI_API_KEY"] = sys.argv[1]
    os.environ["ZAPIER_NLA_API_KEY"] = sys.argv[2]
    que = sys.argv[3]
    
    llm = OpenAI(temperature=.3)
    zapier = ZapierNLAWrapper()
    toolkit = ZapierToolkit.from_zapier_nla_wrapper(zapier)
    agent = initialize_agent(toolkit.get_tools(), llm, agent="zero-shot-react-description", verbose=True)

    ans = hw(que)
    sys.stdout.reconfigure(encoding='utf-8')
    print(ans)




 