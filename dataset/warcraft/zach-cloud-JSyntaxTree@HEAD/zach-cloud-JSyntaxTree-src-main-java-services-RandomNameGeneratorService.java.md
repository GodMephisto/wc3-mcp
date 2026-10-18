---
type: vendor-doc
source_repo: "https://github.com/zach-cloud/JSyntaxTree"
source_path: "src/main/java/services/RandomNameGeneratorService.java"
ref: HEAD
---

package services;

import interfaces.IRandomNameGeneratorService;

import java.util.ArrayList;
import java.util.List;
import java.util.Random;

/**
 * Used to generate random and unique variable/function names
 */
public class RandomNameGeneratorService implements IRandomNameGeneratorService {

    private static final String ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static final int LENGTH = 12;
    private final List<String> usedNames;
    private final Random random;

    /**
     * Initialized the random name generator
     */
    public RandomNameGeneratorService() {
        this.random = new Random();
        this.usedNames = new ArrayList<>();
    }

    /**
     * Produces the next unique random name
     *
     * @return  Unique random name
     */
    @Override
    public String next() {
        StringBuilder name = new StringBuilder();
        for(int i = 0; i < LENGTH; i++) {
            name.append(ALPHABET.charAt(random.nextInt(ALPHABET.length())));
        }
        if(usedNames.contains(name.toString())) {
            return next();
        } else {
            usedNames.add(name.toString());
            return name.toString();
        }
    }
}
