<br />
<p align="center">

  <h3 align="center">News Classification</h3>

  <p align="center">
    A news classfication application that uses three different NLP classfication algorithm.
    <br />
    <br />
    <a href="https://github.com/izzettunc/dbscan/issues">Report Bug</a>
    ·
    <a href="https://github.com/izzettunc/dbscan/issues">Request Feature</a>
  </p>
</p>



<!-- TABLE OF CONTENTS -->
## Table of Contents

* [About the Project](#about-the-project)
  * [Built With](#built-with)
* [Getting Started](#getting-started)
  * [Installation](#installation)
* [Usage](#usage)
* [Roadmap](#roadmap)
* [License](#license)
* [Contact](#contact)



<!-- ABOUT THE PROJECT -->
## About The Project

![Product Name Screen Shot][product-screenshot]

This project made as a class assignment.It's purpose basically classifying given test data by learned information from train data using classification algorithms.I use three different method for compare them between each other.These three algortihms are K-NN -that using four different distance method which them are Pearson Corelation Coefficient,Cosine Similarity,Euler Distance,Manhattan Distance- , Multinomial Naive Bayes and Rocchio -that using same distance methods as K-NN -

**Features:**

* You can classify any given text
* You can visualize train and test process with confusion matrix,performance matrix,accuracy and time metrics
* You can see which document assigned which class by algorithm
* You can print results in a txt file

### What is Multinomial Naive Bayes algorithm ?

In machine learning, naïve Bayes classifiers are a family of simple "probabilistic classifiers" based on applying Bayes' theorem with strong (naïve) independence assumptions between the features. They are among the simplest Bayesian network models. Naïve Bayes has been studied extensively since the 1960s. It was introduced (though not under that name) into the text retrieval community in the early 1960s, and remains a popular (baseline) method for text categorization, the problem of judging documents as belonging to one category or the other (such as spam or legitimate, sports or politics, etc.) with word frequencies as the features. With appropriate pre-processing, it is competitive in this domain with more advanced methods including support vector machines.

Multinomial Naive Bayes is a specialized version of Naive Bayes that is designed more for text documents. Whereas simple naive Bayes would model a document as the presence and absence of particular words, multinomial naive bayes explicitly models the word counts and adjusts the underlying calculations to deal with in.

### What is Rocchio algorithm ?

In machine learning, a nearest centroid classifier or nearest prototype classifier is a classification model that assigns to observations the label of the class of training samples whose mean (centroid) is closest to the observation. When applied to text classification using tf*idf vectors to represent documents, the nearest centroid classifier is known as the Rocchio classifier because of its similarity to the Rocchio algorithm for relevance feedback. An extended version of the nearest centroid classifier has found applications in the medical domain, specifically classification of tumors.

### What is K-NN algorithm ?

In pattern recognition, the k-nearest neighbors algorithm (k-NN) is a non-parametric method used for classification and regression. In both cases, the input consists of the k closest training examples in the feature space. The output depends on whether k-NN is used for classification or regression:

        In k-NN classification, the output is a class membership. An object is classified by a plurality vote of its neighbors, with the object being assigned to the class most common among its k nearest neighbors (k is a positive integer, typically small). If k = 1, then the object is simply assigned to the class of that single nearest neighbor.

        In k-NN regression, the output is the property value for the object. This value is the average of the values of k nearest neighbors.

k-NN is a type of instance-based learning, or lazy learning, where the function is only approximated locally and all computation is deferred until classification.

Both for classification and regression, a useful technique can be to assign weights to the contributions of the neighbors, so that the nearer neighbors contribute more to the average than the more distant ones. For example, a common weighting scheme consists in giving each neighbor a weight of 1/d, where d is the distance to the neighbor.

The neighbors are taken from a set of objects for which the class (for k-NN classification) or the object property value (for k-NN regression) is known. This can be thought of as the training set for the algorithm, though no explicit training step is required.

<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple steps.

### Installation

1.  Clone the repo
```sh
git clone https://github.com/izzettunc/newsClassification.git
```
2. Open **newsClassification.sln** with Visual Studio

3. Make changes, run it, use it whatever you like :smile:

### Important Notice

I use 2 different github project while devoloping this app.If you want ta scrap this project and use classes independetly
make sure to add [BigFloat](https://github.com/izzettunc/dbscan/issues) for Multinomial Naive Bayes and [Nuve](https://github.com/hrzafer/nuve) for tokenizer.

* I use [BigFloat](https://github.com/izzettunc/dbscan/issues) because while calculating ratios and probabilities of documents in MN Bayes result become so small that doesn't fit in double,real or float variable type

* I use [Nuve](https://github.com/hrzafer/nuve) in Tokenizer for stemming Turkish words

**Make sure to add System.Numerics assembly reference to your project if you wanna use [BigFloat](https://github.com/izzettunc/dbscan/issues).Because  BigInteger used while developing [BigFloat](https://github.com/izzettunc/dbscan/issues).**

**Make sure to install [Nuve](https://github.com/hrzafer/nuve) to your project if you wanna use [Nuve](https://github.com/hrzafer/nuve) in your project**

<!-- USAGE EXAMPLES -->
## Usage

Application made in turkish which is my mother tongue.So here is some unneeded screenshots for how to use it

![Application Screen Shot][app-screenshot]

![Result File Screen Shot][result-screenshot]

<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/izzettunc/dbscan/issues) for a list of proposed features (and known issues).

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.

<!-- CONTACT -->
## Contact

İzzet Tunç - izzet.tunc1997@gmail.com
<br>
[![LinkedIn][linkedin-shield]][linkedin-url]

[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=flat-square&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/izzettunc
[product-screenshot]: data/screenshots/dbscan.png
[app-screenshot]: data/screenshots/application.png
[result-screenshot]: data/screenshots/result_file.png
