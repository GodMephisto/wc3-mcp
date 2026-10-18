---
type: vendor-doc
source_repo: "https://github.com/ClaudixoPG/DSI_PCG_War3MapGeneratorAutoencoder"
source_path: "Proyecto/Utilities.py"
ref: HEAD
---

import numpy as np

def transform_shared_values_with_mapping(list_of_lists):
    """
    Transforms multiple lists with a shared ranking system and returns the global mapping
    for converting back to original values.

    Args:
    list_of_lists (list of list): A list containing multiple lists of numerical values

    Returns:
    tuple: A tuple containing the list of transformed lists and the global mapping
    """
    all_values = set()
    for lst in list_of_lists:
        all_values.update(lst)

    all_values_sorted = sorted(all_values)
    value_to_rank = {value: rank + 1 for rank, value in enumerate(all_values_sorted)}
    rank_to_value = {rank + 1: value for rank, value in enumerate(all_values_sorted)}

    transformed_lists = []
    for lst in list_of_lists:
        transformed_list = [value_to_rank[value] for value in lst]
        transformed_lists.append(transformed_list)

    return transformed_lists, rank_to_value

def get_original_values_from_transformed(transformed_list, rank_to_value):
    """
    Retrieves the original values from a transformed list using the provided global mapping.

    Args:
    transformed_list (list): The transformed list.
    rank_to_value (dict): Global mapping from ranks to original values.

    Returns:
    list: The original list of values.
    """
    return [rank_to_value[rank] for rank in transformed_list]

#make a function to stack the numpy arrays list, np.dstack() stacks arrays in sequence depth wise (along third axis) so that depth wise concatenation can be done
def stack_arrays_list(arrays_list):
    return np.dstack(arrays_list)

