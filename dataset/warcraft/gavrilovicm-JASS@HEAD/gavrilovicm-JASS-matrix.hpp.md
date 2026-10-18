---
type: vendor-doc
source_repo: "https://github.com/gavrilovicm/JASS"
source_path: "matrix.hpp"
ref: HEAD
---

#ifndef MATRIX_HPP
#define MATRIX_HPP
#include<iostream>
#include<vector>


class Matrix{
private:
	std::vector<std::vector<double>> m_matrix;
public:
	Matrix(){};
	Matrix(std::vector<std::vector<double>> m);
	std::vector<std::vector<double>> getMatrix();
	void add_to_matrix(std::vector<double> m);

	friend std::ostream& operator<<(std::ostream& out, Matrix m);

};

#endif
