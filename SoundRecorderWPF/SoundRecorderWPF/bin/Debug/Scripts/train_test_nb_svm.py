import json
from random import shuffle, seed
seed(1337)
from sys import argv

from sklearn.pipeline import Pipeline
from sklearn.feature_extraction.text import CountVectorizer
from sklearn.feature_extraction.text import TfidfTransformer
# from sklearn.naive_bayes import MultinomialNB

# from sklearn.linear_model import SGDClassifier
from sklearn import svm
from sklearn.svm import LinearSVC


trainFile = 'trainData.csv'

intentMapDict = {}
trainData = []
with open (trainFile, "r", encoding='utf-8') as fpTrain :
    for line in fpTrain :
        line = line.strip()
        cols = line.split(',')
        text = ' '.join([w for w in cols[0:len(cols) - 2]])
        label = cols[len(cols)-2]
        trainData.append((text, label))
        if (label not in intentMapDict) :
            intentMapDict[label] = cols[len(cols)-1]

# print(trainData)
# print(intentMapDict)

# testData = []
# with open (testFile, "r", encoding='utf-8') as fpTest :
#     for line in fpTest :
#         line = line.strip()
#         cols = line.split(',')
#         text = ' '.join([w for w in cols[0:len(cols) - 1]])
#         label = cols[len(cols)-1]
#         testData.append((text, label))

# text_clf = Pipeline([('vect', CountVectorizer()),
                     # ('tfidf', TfidfTransformer()),
                     # ('clf', MultinomialNB()),])

# text_clf = text_clf.fit([t.lower() for t,l in trainData], [l for t,l in trainData])

# numCorrect = 0
# numIncorrect = 0
# for test, label in testData :
#     pred = text_clf.predict([test.lower()])
#     probs = text_clf.predict_proba([test])
#     if (pred[0] == str(label)) :
#         numCorrect += 1
#     else :
#         numIncorrect += 1

# print(numCorrect, numCorrect/len(testData), numIncorrect, numIncorrect/len(testData), len(testData))

text_clf_svm = Pipeline([('vect', CountVectorizer()),
                     ('tfidf', TfidfTransformer()),
                     ('clf-svm', LinearSVC()),])
text_clf_svm = text_clf_svm.fit([t.lower() for t,l in trainData], [l for t,l in trainData])

# numCorrect = 0
# numIncorrect = 0
# for test, label in testData :
#     pred = text_clf_svm.predict([test.lower()])
#     if (pred[0] == str(label)) :
#         numCorrect += 1
#     else :
#         numIncorrect += 1

# print(numCorrect, numCorrect/len(testData), numIncorrect, numIncorrect/len(testData), len(testData))

# testData = input()
# while (testData != 'quit') :
    # print(testData, intentMapDict[text_clf_svm.predict([testData])[0]], sep=',')
    # print()
    # testData = input()
    
testData = argv[1]
print(intentMapDict[text_clf_svm.predict([testData])[0]])



