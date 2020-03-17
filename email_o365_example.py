from O365 import Account
credentials = ('id', 'secret')
scopes = ['https://graph.microsoft.com/Mail.ReadWrite', 'https://graph.microsoft.com/Mail.Send']
account = Account(credentials, scopes=scopes)
account.authenticate()
m = account.new_message()
m.to.add('peterfromswe884@gmail.com')
m.subject = 'Testing!'
m.body = "George Best quote: I've stopped drinking, but only while I'm asleep."
m.send()
